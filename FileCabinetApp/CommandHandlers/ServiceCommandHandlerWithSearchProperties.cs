using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract service command handler with properties.
    /// </summary>
    public abstract class ServiceCommandHandlerWithSearchProperties : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Where word.
        /// </summary>
        protected const string WhereSeparator = "WHERE";

        /// <summary>
        /// And.
        /// </summary>
        protected const string SignAnd = "AND";

        /// <summary>
        /// Or.
        /// </summary>
        protected const string SignOr = "OR";

        /// <summary>
        /// Sign group.
        /// </summary>
        protected const string SignGroupName = "sign";

        private const string FieldGroupName = "field";
        private const string ValueGroupName = "value";
        private const string RecordGroupName = "record";

        private readonly Regex propertiesRegex;
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerWithSearchProperties"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        protected ServiceCommandHandlerWithSearchProperties(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
            var countOfSymbol = @"{2,}";
            var availableSymbols = @"' .,\\\/";
            this.propertiesRegex = new Regex($" *(?<{RecordGroupName}>(?<{FieldGroupName}>[a-zA-Z]{countOfSymbol}) *= *" +
                                             $"'(?<{ValueGroupName}>[a-zA-Z0-9{availableSymbols}]*)' *(?<{SignGroupName}>($|AND|OR)))+");
        }

        /// <summary>
        /// Returns true if actual fields added to list.
        /// </summary>
        /// <typeparam name="T">Type of field.</typeparam>
        /// <param name="expectedFieldName">Expected field name.</param>
        /// <param name="actualFieldName">Actual field name.</param>
        /// <param name="value">Actual value.</param>
        /// <param name="list">List for adding fields.</param>
        /// <param name="converter">Convereter.</param>
        /// <param name="validator">Validator.</param>
        /// <returns>True if actual fields added to list.</returns>
        protected static bool TryAddFieldToList<T>(string expectedFieldName, string actualFieldName, string value, List<Tuple<string, object>> list, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter), "Converter cannot be null.");
            }

            if (actualFieldName == expectedFieldName)
            {
                var converted = converter(value);
                if (converted.Item1)
                {
                    var validated = validator(converted.Item3);
                    if (validated.Item1)
                    {
                        list.Add(new Tuple<string, object>(expectedFieldName, converted.Item3));
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Validation '{expectedFieldName}' failed: {validated.Item2}.");
                    }
                }
                else
                {
                    Console.WriteLine($"Conversion '{expectedFieldName}' of '{value}' failed: {converted.Item2}.");
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if properties and signs fields successfully were gotten.
        /// </summary>
        /// <param name="propertiesString">String with properties.</param>
        /// <param name="searchPropertiesList">Search properties list.</param>
        /// <param name="searchPropertiesSignList">Search properties sign list.</param>
        /// <returns>True if properties and signs fields successfully were gotten.</returns>
        protected bool TryGetPropertiesAndSigns(string propertiesString, out List<Tuple<string, object>> searchPropertiesList, out List<string> searchPropertiesSignList)
        {
            searchPropertiesSignList = new List<string>();
            searchPropertiesList = new List<Tuple<string, object>>();
            var searchPropertiesMatches = this.propertiesRegex.Matches(propertiesString);
            if (searchPropertiesMatches.Any() && IsLastSignCorrect(searchPropertiesMatches[^1].Groups[SignGroupName].Value))
            {
                foreach (Match searchPropertiesMatch in searchPropertiesMatches)
                {
                    var fieldName = searchPropertiesMatch.Groups[FieldGroupName].Value;
                    var value = searchPropertiesMatch.Groups[ValueGroupName].Value;
                    if (!TryAddFieldToList(nameof(FileCabinetRecord.Id).ToUpperInvariant(), fieldName, value, searchPropertiesList, IntConverter, IdentifierValidator) &&
                        !TryAddFieldToList(nameof(FileCabinetRecord.FirstName).ToUpperInvariant(), fieldName, value, searchPropertiesList, StringConverter, this.FirstNameValidator) &&
                        !TryAddFieldToList(nameof(FileCabinetRecord.LastName).ToUpperInvariant(), fieldName, value, searchPropertiesList, StringConverter, this.LastNameValidator) &&
                        !TryAddFieldToList(nameof(FileCabinetRecord.DateOfBirth).ToUpperInvariant(), fieldName, value, searchPropertiesList, DateConverter, this.DateValidator) &&
                        !TryAddFieldToList(nameof(FileCabinetRecord.Height).ToUpperInvariant(), fieldName, value, searchPropertiesList, ShortConverter, this.AgeValidator) &&
                        !TryAddFieldToList(nameof(FileCabinetRecord.Wallet).ToUpperInvariant(), fieldName, value, searchPropertiesList, DecimalConverter, this.BudgetValidator) &&
                        !TryAddFieldToList(nameof(FileCabinetRecord.MaritalStatus).ToUpperInvariant(), fieldName, value, searchPropertiesList, CharConverter, this.MaritalStatusValidator))
                    {
                        Console.WriteLine("Error..." + Environment.NewLine + "Correct the entered data and repeat.");
                        return false;
                    }

                    searchPropertiesSignList.Add(searchPropertiesMatch.Groups[SignGroupName].Value);
                    propertiesString = propertiesString.Replace(searchPropertiesMatch.Groups[RecordGroupName].Value, string.Empty, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            return IsPropertiesValid(propertiesString, searchPropertiesSignList.Count, searchPropertiesList.Count);
        }

        private static bool IsPropertiesValid(string propertiesString, int countOfSigns, int countOfProperties) =>
            propertiesString.Trim().Length == 0 && countOfSigns == countOfProperties;

        private static bool IsLastSignCorrect(string lastSign) => lastSign != SignAnd && lastSign != SignOr;
    }
}
