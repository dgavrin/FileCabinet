using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Select command handler.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase, ICommandHandler
    {
        private const string Command = "select";

        private ICommandHandler nextHandler;
        private Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        /// <param name="printer">Printer.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer ?? throw new ArgumentNullException(nameof(printer));
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Select(appCommandRequest.Parameters);
            }
            else
            {
                this.nextHandler.Handle(appCommandRequest);
            }
        }

        /// <inheritdoc/>
        public new void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
        }

        private static List<KeyValuePair<string, string>> GetKeyValuePairsOfSearchCriteria(string searchCriteria, out string logicalOperation)
        {
            const int keyIndex = 1;
            const int valueIndex = 2;
            const string InvalidOperatorsMessage = "Incorrect use of the 'and' and 'or' operators.";

            if (searchCriteria == null)
            {
                throw new ArgumentNullException(nameof(searchCriteria));
            }

            logicalOperation = string.Empty;

            if (searchCriteria.Length == 0 || string.IsNullOrWhiteSpace(searchCriteria))
            {
                return new List<KeyValuePair<string, string>>();
            }

            var andRegex = new Regex("and", RegexOptions.IgnoreCase);
            var orRegex = new Regex("or", RegexOptions.IgnoreCase);

            if (andRegex.IsMatch(searchCriteria))
            {
                if (orRegex.IsMatch(searchCriteria))
                {
                    throw new ArgumentException(InvalidOperatorsMessage);
                }
                else
                {
                    logicalOperation = "and";
                }
            }
            else
            {
                if (orRegex.IsMatch(searchCriteria))
                {
                    logicalOperation = "or";
                }
                else
                {
                    throw new ArgumentException(InvalidOperatorsMessage);
                }
            }

            var keyValuePairs = new List<KeyValuePair<string, string>>();
            var separatedSearchCriteria = searchCriteria.Split($" {logicalOperation} ");
            var criteriaRegex = new Regex(@"(.*)=(.*)");

            foreach (var criteria in separatedSearchCriteria)
            {
                if (criteriaRegex.IsMatch(criteria))
                {
                    var match = criteriaRegex.Match(criteria);
                    var key = match.Groups[keyIndex].Value.Trim(' ').ToUpperInvariant();
                    var value = Regex.Match(match.Groups[valueIndex].Value, @"'(.*)'").Groups[1].Value.Trim(' ');
                    keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
                }
                else
                {
                    throw new ArgumentException("One of the entered search criteria is incorrect.");
                }
            }

            if (logicalOperation.Equals("and", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var keyValuePair in keyValuePairs)
                {
                    if (keyValuePairs.FindAll(pair => pair.Key == keyValuePair.Key).Count > 1)
                    {
                        throw new ArgumentException("When using 'and' it is not possible to use different values for the same search keys.");
                    }
                }
            }

            return keyValuePairs;
        }

        private void Select(string parameters)
        {
            const int selectedAttributesIndex = 1;
            const int searchCriteriaIndex = 2;
            const string invalidCommandSyntaxMessage = "Incorrect syntax for 'select' command.";

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters));
                }

                var parametersRegex = new Regex(@"(.*)where(.*)$");
                if (parametersRegex.IsMatch(parameters))
                {
                    var matchParameters = parametersRegex.Match(parameters);
                    var selectedAttributes = matchParameters.Groups[selectedAttributesIndex].Value.ToUpperInvariant().Split(", ").ToList();
                    var searchCriteria = GetKeyValuePairsOfSearchCriteria(matchParameters.Groups[searchCriteriaIndex].Value, out string logicalOperation);
                    for (int i = 0; i < selectedAttributes.Count; i++)
                    {
                        selectedAttributes[i] = selectedAttributes[i].Trim(' ');
                        if (selectedAttributes[i].Length == 0)
                        {
                            selectedAttributes.RemoveAt(i);
                        }
                    }

                    IEnumerable<FileCabinetRecord> selectedRecords;
                    if (searchCriteria.Count == 0)
                    {
                        selectedRecords = this.fileCabinetService.GetRecords();
                    }
                    else
                    {
                        selectedRecords = this.fileCabinetService.SelectByCriteria(searchCriteria, logicalOperation);
                    }

                    TablePrinter.Print(selectedRecords, selectedAttributes);
                    Console.WriteLine();
                }
                else
                {
                    throw new ArgumentException(invalidCommandSyntaxMessage);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Examples 'select' command:");
                Console.WriteLine("select id, firstname, lastname where firstname = 'John' and lastname = 'Doe';");
                Console.WriteLine();
            }
        }

        private static class TablePrinter
        {
            private const string CrossingLines = "+";
            private const string HorizontalLine = "-";
            private const string VerticalLine = "|";

            private static readonly List<string> NamesOfRecordFields = new List<string>
            {
                "ID",
                "FIRSTNAME",
                "LASTNAME",
                "DATEOFBIRTH",
                "WALLET",
                "MARITALSTATUS",
                "HEIGHT",
            };

            private static readonly Dictionary<string, int> MinimalFieldsLength = new Dictionary<string, int>
            {
                { "ID", 2 },
                { "FIRSTNAME", 9 },
                { "LASTNAME", 8 },
                { "DATEOFBIRTH", 11 },
                { "WALLET", 6 },
                { "MARITALSTATUS", 13 },
                { "HEIGHT", 6 },
            };

            /// <summary>
            /// Prints the records with specified fields in table view.
            /// </summary>
            /// <param name="records">The records to be printed.</param>
            /// <param name="recordFields">The fields of records to print.</param>
            public static void Print(IEnumerable<FileCabinetRecord> records, List<string> recordFields)
            {
                if (records == null)
                {
                    throw new ArgumentNullException(nameof(records));
                }

                if (recordFields == null)
                {
                    throw new ArgumentNullException(nameof(recordFields));
                }

                var fieldsLength = GetMaxFieldsLength(records);
                var horizontalTableLine = CreateHorizontalTableLine(recordFields, fieldsLength);

                Console.WriteLine(horizontalTableLine);
                Console.WriteLine(CreateTableHead(recordFields, fieldsLength));
                Console.WriteLine(horizontalTableLine);

                foreach (var record in records)
                {
                    Console.WriteLine(CreateTableLine(record, recordFields, fieldsLength));
                }

                Console.WriteLine(horizontalTableLine);
            }

            private static string CreateTableLine(FileCabinetRecord record, List<string> recordFields, Dictionary<string, int> fieldsLength)
            {
                if (record == null)
                {
                    throw new ArgumentNullException(nameof(record));
                }

                if (recordFields == null)
                {
                    throw new ArgumentNullException(nameof(recordFields));
                }

                if (fieldsLength == null)
                {
                    throw new ArgumentNullException(nameof(fieldsLength));
                }

                var tableLine = new StringBuilder();
                tableLine.Append(VerticalLine);

                foreach (var fieldLength in fieldsLength)
                {
                    if (recordFields.Count > 0 && !recordFields.Contains(fieldLength.Key))
                    {
                        continue;
                    }
                    else
                    {
                        switch (fieldLength.Key)
                        {
                            case "ID":
                                tableLine.Append($" {record.Id.ToString(new CultureInfo("en-US")).PadLeft(fieldLength.Value)} ");

                                break;
                            case "FIRSTNAME":
                                tableLine.Append($" {record.FirstName.PadRight(fieldLength.Value)} ");

                                break;
                            case "LASTNAME":
                                tableLine.Append($" {record.LastName.PadRight(fieldLength.Value)} ");

                                break;
                            case "DATEOFBIRTH":
                                tableLine.Append($" {record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US")).PadLeft(fieldLength.Value)} ");

                                break;
                            case "WALLET":
                                tableLine.Append($" {record.Wallet.ToString(new CultureInfo("en-US")).PadLeft(fieldLength.Value)} ");

                                break;
                            case "MARITALSTATUS":
                                var maritalStatus = "unmarried";
                                if (record.MaritalStatus == 'm' || record.MaritalStatus == 'M')
                                {
                                    maritalStatus = "married";
                                }

                                tableLine.Append($" {maritalStatus.PadRight(fieldLength.Value)} ");

                                break;
                            case "HEIGHT":
                                tableLine.Append($" {record.Height.ToString(new CultureInfo("en-US")).PadLeft(fieldLength.Value)} ");

                                break;
                        }

                        tableLine.Append(VerticalLine);
                    }
                }

                return tableLine.ToString();
            }

            private static string CreateTableHead(List<string> recordFields, Dictionary<string, int> fieldsLength)
            {
                if (recordFields == null)
                {
                    throw new ArgumentNullException(nameof(recordFields));
                }

                if (fieldsLength == null)
                {
                    throw new ArgumentNullException(nameof(fieldsLength));
                }

                var tableHead = new StringBuilder();
                tableHead.Append(VerticalLine);

                foreach (var fieldLength in fieldsLength)
                {
                    if (recordFields.Count > 0 && !recordFields.Contains(fieldLength.Key))
                    {
                        continue;
                    }
                    else
                    {
                        tableHead.Append($" {fieldLength.Key.PadRight(fieldLength.Value)} {VerticalLine}");
                    }
                }

                return tableHead.ToString();
            }

            private static string CreateHorizontalTableLine(List<string> recordFields, Dictionary<string, int> fieldsLength)
            {
                if (recordFields == null)
                {
                    throw new ArgumentNullException(nameof(recordFields));
                }

                if (fieldsLength == null)
                {
                    throw new ArgumentNullException(nameof(fieldsLength));
                }

                var horizontalTableLine = new StringBuilder();
                horizontalTableLine.Append(CrossingLines + HorizontalLine);
                for (int i = 0; i < fieldsLength.Count; i++)
                {
                    if (recordFields.Count > 0 && !recordFields.Contains(fieldsLength.ElementAt(i).Key))
                    {
                        continue;
                    }
                    else
                    {
                        var recordFieldLength = fieldsLength.ElementAt(i).Value;
                        for (int j = 0; j < recordFieldLength; j++)
                        {
                            horizontalTableLine.Append(HorizontalLine);
                        }

                        horizontalTableLine.Append(HorizontalLine + CrossingLines);
                        if (i < fieldsLength.Count - 1)
                        {
                            horizontalTableLine.Append(HorizontalLine);
                        }
                    }
                }

                var resultHorizontalTableLine = horizontalTableLine.ToString();
                if (resultHorizontalTableLine[resultHorizontalTableLine.Length - 1] == Convert.ToChar(HorizontalLine, new CultureInfo("en-US")))
                {
                    resultHorizontalTableLine = resultHorizontalTableLine.Remove(resultHorizontalTableLine.Length - 1);
                }

                return resultHorizontalTableLine;
            }

            private static Dictionary<string, int> GetMaxFieldsLength(IEnumerable<FileCabinetRecord> records)
            {
                var maxFieldsLength = new Dictionary<string, int>();

                foreach (var nameOfRecordField in NamesOfRecordFields)
                {
                    switch (nameOfRecordField)
                    {
                        case "ID":
                            int maxIdLegth = records.Max(record => record.Id.ToString(new CultureInfo("en-US")).Length);
                            maxIdLegth = MinimalFieldsLength["ID"] > maxIdLegth ? MinimalFieldsLength["ID"] : maxIdLegth;
                            maxFieldsLength.Add("ID", maxIdLegth);

                            break;
                        case "FIRSTNAME":
                            int maxFirstNameLegth = records.Max(record => record.FirstName.Length);
                            maxFirstNameLegth = MinimalFieldsLength["FIRSTNAME"] > maxFirstNameLegth ? MinimalFieldsLength["FIRSTNAME"] : maxFirstNameLegth;
                            maxFieldsLength.Add("FIRSTNAME", maxFirstNameLegth);

                            break;
                        case "LASTNAME":
                            int maxLastNameLegth = records.Max(record => record.LastName.Length);
                            maxLastNameLegth = MinimalFieldsLength["LASTNAME"] > maxLastNameLegth ? MinimalFieldsLength["LASTNAME"] : maxLastNameLegth;
                            maxFieldsLength.Add("LASTNAME", maxLastNameLegth);

                            break;
                        case "DATEOFBIRTH":
                            int maxDateOfBirthLength = records.Max(record => record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US")).Length);
                            maxDateOfBirthLength = MinimalFieldsLength["DATEOFBIRTH"] > maxDateOfBirthLength ? MinimalFieldsLength["DATEOFBIRTH"] : maxDateOfBirthLength;
                            maxFieldsLength.Add("DATEOFBIRTH", MinimalFieldsLength["DATEOFBIRTH"]);

                            break;
                        case "WALLET":
                            int maxWalletLength = records.Max(record => record.Wallet.ToString(new CultureInfo("en-US")).Length);
                            maxWalletLength = MinimalFieldsLength["WALLET"] > maxWalletLength ? MinimalFieldsLength["WALLET"] : maxWalletLength;
                            maxFieldsLength.Add("WALLET", maxWalletLength);

                            break;
                        case "MARITALSTATUS":
                            maxFieldsLength.Add("MARITALSTATUS", MinimalFieldsLength["MARITALSTATUS"]);

                            break;
                        case "HEIGHT":
                            int maxHeightLength = records.Max(record => record.Height.ToString(new CultureInfo("en-US")).Length);
                            maxHeightLength = MinimalFieldsLength["HEIGHT"] > maxHeightLength ? MinimalFieldsLength["HEIGHT"] : maxHeightLength;
                            maxFieldsLength.Add("HEIGHT", maxHeightLength);

                            break;
                    }
                }

                return maxFieldsLength;
            }
        }
    }
}
