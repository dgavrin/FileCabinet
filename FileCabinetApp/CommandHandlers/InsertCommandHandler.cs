using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Insert command handler.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase, ICommandHandler
    {
        private const string Command = "insert";

        private ICommandHandler nextHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
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
                this.Insert(appCommandRequest.Parameters);
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

        private void Insert(string parameters)
        {
            const int attributeIndex = 1;
            const int attributeValueIndex = 2;
            const int recordParametersCount = 7;
            const string checkEnteredvaluesMessage = "Check the entered values, they should follow in the same order as the parameters.";

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters), "The list of parameters for the 'insert' command cannot be empty.");
                }

                var parameterRegex = new Regex(@"(.*) values (.*)", RegexOptions.IgnoreCase);
                if (parameterRegex.IsMatch(parameters))
                {
                    var matchParameters = parameterRegex.Match(parameters);
                    var attribute = matchParameters.Groups[attributeIndex].Value.ToUpperInvariant();
                    var attributeValue = matchParameters.Groups[attributeValueIndex].Value;
                    var fileCabinetRecordFields = GetSeparatedStrings(attribute);
                    var valuesFileCabinetRecodrFields = GetSeparatedStrings(attributeValue);

                    if (!(fileCabinetRecordFields.Count == valuesFileCabinetRecodrFields.Count && fileCabinetRecordFields.Count == recordParametersCount))
                    {
                        throw new ArgumentException($"The number of required parameters is {recordParametersCount}.");
                    }

                    var recordForInsert = new FileCabinetRecord();
                    for (int i = 0; i < fileCabinetRecordFields.Count; i++)
                    {
                        switch (fileCabinetRecordFields[i])
                        {
                            case "ID":
                                if (int.TryParse(valuesFileCabinetRecodrFields[i], out int id))
                                {
                                    recordForInsert.Id = id;
                                }
                                else
                                {
                                    throw new ArgumentException(checkEnteredvaluesMessage);
                                }

                                break;
                            case "FIRSTNAME":
                                recordForInsert.FirstName = valuesFileCabinetRecodrFields[i];

                                break;
                            case "LASTNAME":
                                recordForInsert.LastName = valuesFileCabinetRecodrFields[i];

                                break;
                            case "DATEOFBIRTH":
                                if (DateTime.TryParse(valuesFileCabinetRecodrFields[i], new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth))
                                {
                                    recordForInsert.DateOfBirth = dateOfBirth;
                                }
                                else
                                {
                                    throw new ArgumentException(checkEnteredvaluesMessage);
                                }

                                break;
                            case "WALLET":
                                if (decimal.TryParse(valuesFileCabinetRecodrFields[i], out decimal wallet))
                                {
                                    recordForInsert.Wallet = wallet;
                                }
                                else
                                {
                                    throw new ArgumentException(checkEnteredvaluesMessage);
                                }

                                break;
                            case "MARITALSTATUS":
                                if (char.TryParse(valuesFileCabinetRecodrFields[i], out char maritalStatus))
                                {
                                    recordForInsert.MaritalStatus = maritalStatus;
                                }
                                else
                                {
                                    throw new ArgumentException(checkEnteredvaluesMessage);
                                }

                                break;
                            case "HEIGHT":
                                if (short.TryParse(valuesFileCabinetRecodrFields[i], out short height))
                                {
                                    recordForInsert.Height = height;
                                }
                                else
                                {
                                    throw new ArgumentException(checkEnteredvaluesMessage);
                                }

                                break;
                            default:
                                throw new ArgumentException(checkEnteredvaluesMessage);
                        }
                    }

                    var insertedRecordId = this.fileCabinetService.Insert(recordForInsert);
                    Console.WriteLine($"Record #{insertedRecordId} inserted.");
                    Console.WriteLine();
                }
                else
                {
                    throw new ArgumentException("Incorrect syntax for 'insert' command.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"No entry has been inserted. {ex.Message}");
                Console.WriteLine("Example 'insert' command: insert (id, firstname, lastname, dateofbirth, wallet, maritalstatus, height) values (1, Salvador, Harris, 11.11.1990, 100, u, 180)");
                Console.WriteLine();
            }

            List<string> GetSeparatedStrings(string inputString)
            {
                var separatedStrings = inputString.Split(new char[] { '(', ',', ' ', ')' }).ToList<string>();
                separatedStrings.RemoveAll(x => string.IsNullOrEmpty(x));
                return separatedStrings;
            }
        }
    }
}
