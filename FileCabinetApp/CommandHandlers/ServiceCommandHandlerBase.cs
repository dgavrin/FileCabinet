using System;
using FileCabinetApp.Records;
using FileCabinetApp.Services;
using FileCabinetApp.Validators.InputValidator;

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
