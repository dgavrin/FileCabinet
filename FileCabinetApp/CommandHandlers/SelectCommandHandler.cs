using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Select command handler.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerWithSearchProperties, ICommandHandler
    {
        private const string Command = "select";
        private const string DisplayedFieldsGroupName = "fields";
        private const string SearchPropertiesGroupName = "searchProperties";
        private const string WordToDisplayAllFields = "ALL";

        private static readonly string HintMessage =
            "Incorrect format command." +
            Environment.NewLine +
            "Try: select firstname where id = '1'" +
            Environment.NewLine +
            "'all' - to display all fields of the record";

        private readonly Regex selectCommandWithSearchPropertiesRegex;
        private readonly Regex selectCommandWithoutSearchPropertiesRegex;

        private ICommandHandler nextHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.selectCommandWithSearchPropertiesRegex = new Regex(
                $"^ *(?<{DisplayedFieldsGroupName}>[a-zA-Z0-9\\W]+) *{WhereSeparator} *" +
                $"(?<{SearchPropertiesGroupName}>[a-zA-Z0-9\\W]+) *$");

            this.selectCommandWithoutSearchPropertiesRegex = new Regex(
                $"(?<{DisplayedFieldsGroupName}>{nameof(FileCabinetRecord.Id).ToUpperInvariant()}|" +
                $"{nameof(FileCabinetRecord.FirstName).ToUpperInvariant()}|" +
                $"{nameof(FileCabinetRecord.LastName).ToUpperInvariant()}|" +
                $"{nameof(FileCabinetRecord.DateOfBirth).ToUpperInvariant()}|" +
                $"{nameof(FileCabinetRecord.Wallet).ToUpperInvariant()}|" +
                $"{nameof(FileCabinetRecord.MaritalStatus).ToUpperInvariant()}|" +
                $"{nameof(FileCabinetRecord.Height).ToUpperInvariant()}|{WordToDisplayAllFields}) *" +
                $"(?<{SignGroupName}>,|$)");
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

        private static bool IsDisplayedFieldAreValid(string parameters, MatchCollection fieldsMatches) =>
            parameters.Trim().Length == 0 && fieldsMatches[^1].Groups[SignGroupName].Value == new string(string.Empty);

        private void Select(string parameters)
        {
            var isNoRecordsFound = false;
            parameters = parameters.ToUpperInvariant();
            IEnumerable<FileCabinetRecord> selectedRecords = new List<FileCabinetRecord>();
            List<string> displayedFieldsCollection;
            if (this.TrySelectRecordsWithoutProperties(parameters, out displayedFieldsCollection))
            {
                selectedRecords = this.fileCabinetService.GetRecords().ToList();
                isNoRecordsFound = !selectedRecords.Any();
            }
            else if (this.TrySelectRecordsWithProperties(parameters, out displayedFieldsCollection, out List<string> propertiesSignList, out List<Tuple<string, object>> propertiesList))
            {
                ReadOnlyCollection<Tuple<string, object>> readonlyPropertyList = new ReadOnlyCollection<Tuple<string, object>>(propertiesList);
                ReadOnlyCollection<string> readOnlyPropertySignsList = new ReadOnlyCollection<string>(propertiesSignList);
                SearchProperties searchProperties = new SearchProperties(readonlyPropertyList, readOnlyPropertySignsList);
                if (!(searchProperties is null))
                {
                    selectedRecords = Memoizer.GetMemoizer(this.fileCabinetService).Select(searchProperties);
                    isNoRecordsFound = !selectedRecords.Any();
                }
            }

            if (selectedRecords.Any())
            {
                TablePrinter.Print(selectedRecords.OrderBy(record => record.Id), displayedFieldsCollection);
                Console.WriteLine();
                return;
            }
            else if (isNoRecordsFound)
            {
                Console.WriteLine("Records with this criteria not found.");
                Console.WriteLine();
                return;
            }

            Console.WriteLine(HintMessage);
            Console.WriteLine();
        }

        private bool TrySelectRecordsWithProperties(string parameters, out List<string> displayedFieldsCollection, out List<string> searchPropertiesSignList, out List<Tuple<string, object>> searchPropertiesList)
        {
            displayedFieldsCollection = new List<string>();
            searchPropertiesSignList = new List<string>();
            searchPropertiesList = new List<Tuple<string, object>>();
            if (this.selectCommandWithSearchPropertiesRegex.IsMatch(parameters))
            {
                var command = this.selectCommandWithSearchPropertiesRegex.Match(parameters);
                var displayedFields = command.Groups[DisplayedFieldsGroupName].Value;
                var searchProperties = command.Groups[SearchPropertiesGroupName].Value;
                return this.selectCommandWithoutSearchPropertiesRegex.IsMatch(displayedFields) &&
                       this.TryGetDisplayedFieldsFromParametersString(displayedFields, out displayedFieldsCollection) &&
                       this.TryGetPropertiesAndSigns(searchProperties, out searchPropertiesList, out searchPropertiesSignList);
            }

            return false;
        }

        private bool TrySelectRecordsWithoutProperties(string parameters, out List<string> displayedFieldsCollection)
        {
            displayedFieldsCollection = new List<string>();
            return this.selectCommandWithoutSearchPropertiesRegex.IsMatch(parameters) &&
                   !parameters.Contains(WhereSeparator, StringComparison.InvariantCultureIgnoreCase) &&
                   this.TryGetDisplayedFieldsFromParametersString(parameters, out displayedFieldsCollection);
        }

        private bool TryGetDisplayedFieldsFromParametersString(string displayedFieldsString, out List<string> displayedFieldsCollection)
        {
            displayedFieldsCollection = new List<string>();
            var displayedFieldsMatches = this.selectCommandWithoutSearchPropertiesRegex.Matches(displayedFieldsString);
            foreach (Match displayedFieldsMatch in displayedFieldsMatches)
            {
                string fieldName = displayedFieldsMatch.Groups[DisplayedFieldsGroupName].Value;
                if (!displayedFieldsCollection.Contains(fieldName))
                {
                    displayedFieldsCollection.Add(fieldName);
                }

                displayedFieldsString = displayedFieldsString.Replace(displayedFieldsMatch.Value, string.Empty, StringComparison.InvariantCultureIgnoreCase);
            }

            if (displayedFieldsCollection.Contains(WordToDisplayAllFields))
            {
                displayedFieldsCollection = new List<string>()
                {
                    nameof(FileCabinetRecord.Id).ToUpperInvariant(),
                    nameof(FileCabinetRecord.FirstName).ToUpperInvariant(),
                    nameof(FileCabinetRecord.LastName).ToUpperInvariant(),
                    nameof(FileCabinetRecord.DateOfBirth).ToUpperInvariant(),
                    nameof(FileCabinetRecord.Wallet).ToUpperInvariant(),
                    nameof(FileCabinetRecord.MaritalStatus).ToUpperInvariant(),
                    nameof(FileCabinetRecord.Height).ToUpperInvariant(),
                };
            }

            return IsDisplayedFieldAreValid(displayedFieldsString, displayedFieldsMatches);
        }
    }
}
