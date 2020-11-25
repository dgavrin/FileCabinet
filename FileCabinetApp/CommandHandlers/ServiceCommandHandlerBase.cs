using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// A base of command handlers that use IFileCabinetService.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// FileCabinetService.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Не объявляйте видимые поля экземпляров", Justification = "<Ожидание>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<Ожидание>")]
        protected IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        /// <summary>
        /// Gets current string converter.
        /// </summary>
        /// <value>
        /// Current string converter.
        /// </value>
        protected static Func<string, Tuple<bool, string, string>> StringConverter => InputConverters.StringConverter;

        /// <summary>
        /// Gets current datetime converter.
        /// </summary>
        /// <value>
        /// Current datetime converter.
        /// </value>
        protected static Func<string, Tuple<bool, string, DateTime>> DateConverter => InputConverters.DateConverter;

        /// <summary>
        /// Gets current char converter.
        /// </summary>
        /// <value>
        /// Current char converter.
        /// </value>
        protected static Func<string, Tuple<bool, string, char>> CharConverter => InputConverters.MaritalStatusConverter;

        /// <summary>
        /// Gets current short converter.
        /// </summary>
        /// <value>
        /// Current short converter.
        /// </value>
        protected static Func<string, Tuple<bool, string, short>> ShortConverter => InputConverters.HeightConverter;

        /// <summary>
        /// Gets current int converter.
        /// </summary>
        /// <value>
        /// Current int converter.
        /// </value>
        protected static Func<string, Tuple<bool, string, int>> IntConverter => InputConverters.IntConverter;

        /// <summary>
        /// Gets current decimal converter.
        /// </summary>
        /// <value>
        /// Current decimal converter.
        /// </value>
        protected static Func<string, Tuple<bool, string, decimal>> DecimalConverter => InputConverters.WalletConverter;

        /// <summary>
        /// Gets validator for cancelling.
        /// </summary>
        /// <value>Validator for cancelling.</value>
        protected static Func<char, Tuple<bool, string>> CancellingValidator => (char chr) =>
        {
            bool isValid = true;
            string message = new string(string.Empty);
            if (!(chr == 'y' || chr == 'Y' ||
                chr == 'n' || chr == 'N'))
            {
                isValid = false;
                message = $"Expected 'Y' or 'N'";
            }

            return new Tuple<bool, string>(isValid, message);
        };

        /// <summary>
        /// Gets validator for id.
        /// </summary>
        /// <value>Validator for id.</value>
        protected static Func<int, Tuple<bool, string>> IdentifierValidator => (int value) =>
        {
            bool isValid = true;
            string message = new string(string.Empty);
            if (value < 1)
            {
                isValid = false;
                message = $"{nameof(FileCabinetRecord.Id)} can't be less than 1. Current {nameof(FileCabinetRecord.Id)} = {value}";
            }

            return new Tuple<bool, string>(isValid, message);
        };

        /// <summary>
        /// Gets validator for firstname.
        /// </summary>
        /// <value>Validator for firstname.</value>
        protected Func<string, Tuple<bool, string>> FirstNameValidator => this.fileCabinetService.InputValidator.FirstNameValidator;

        /// <summary>
        /// Gets validator for lastname.
        /// </summary>
        /// <value>Validator for lastname.</value>
        protected Func<string, Tuple<bool, string>> LastNameValidator => this.fileCabinetService.InputValidator.LastNameValidator;

        /// <summary>
        /// Gets validator for date of birth.
        /// </summary>
        /// <value>Validator for date of birth.</value>
        protected Func<DateTime, Tuple<bool, string>> DateValidator => this.fileCabinetService.InputValidator.DateOfBirthValidator;

        /// <summary>
        /// Gets validator for age.
        /// </summary>
        /// <value>Validator for age.</value>
        protected Func<short, Tuple<bool, string>> AgeValidator => this.fileCabinetService.InputValidator.HeightValidator;

        /// <summary>
        /// Gets validator for marital status.
        /// </summary>
        /// <value>Validator for marital status.</value>
        protected Func<char, Tuple<bool, string>> MaritalStatusValidator => this.fileCabinetService.InputValidator.MaritalStatusValidator;

        /// <summary>
        /// Gets validator for budget.
        /// </summary>
        /// <value>Validator for budget.</value>
        protected Func<decimal, Tuple<bool, string>> BudgetValidator => this.fileCabinetService.InputValidator.WalletValidator;

        /// <summary>
        /// Enter personal information about the person to record.
        /// </summary>
        /// <returns> RecordParameters. </returns>
        protected RecordParameters SetInformationToRecord()
        {
            var inputValidator = this.fileCabinetService.InputValidator;

            Console.Write("First Name: ");
            var firstName = ReadInput(InputConverters.StringConverter, inputValidator.FirstNameValidator);

            Console.Write("Last Name: ");
            var lastName = ReadInput(InputConverters.StringConverter, inputValidator.LastNameValidator);

            Console.Write("Date of birth (MM/DD/YYYY): ");
            var dateOfBirth = ReadInput(InputConverters.DateConverter, inputValidator.DateOfBirthValidator);

            Console.Write("Wallet: ");
            var wallet = ReadInput(InputConverters.WalletConverter, inputValidator.WalletValidator);

            Console.Write("Marital status ('M' - married, 'U' - unmarried): ");
            var maritalStatus = ReadInput(InputConverters.MaritalStatusConverter, inputValidator.MaritalStatusValidator);

            Console.Write("Height: ");
            var height = ReadInput(InputConverters.HeightConverter, inputValidator.HeightValidator);

            return new RecordParameters(firstName, lastName, dateOfBirth, wallet, maritalStatus, height);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
