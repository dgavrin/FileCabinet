using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents the default service, which stores records with personal information about a person.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        private const short MinimumHeight = 0;
        private const int MinimumLengthOfFirstAndLastName = 2;
        private const int MaximumLengthOfFirstAndLastName = 60;
        private const decimal MinimumAmountOfMoney = 0M;
        private static readonly DateTime MinimalDateOfBirth = new DateTime(1950, 1, 1);

        /// <inheritdoc/>
        public override RecordParameters SetInformationToRecord()
        {
            const int informationAboutMaritalStatus = 0;

            Console.Write("First Name: ");
            var firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            var lastName = Console.ReadLine();

            Console.Write("Date of birth (MM/DD/YYYY): ");
            var dateOfBirth = DateTime.Parse(Console.ReadLine(), CultureEnUS);

            Console.WriteLine($"Wallet (from {MinimumAmountOfMoney}): ");
            var wallet = decimal.Parse(Console.ReadLine(), CultureEnUS);

            Console.WriteLine("Marital status ('M' - married, 'U' - unmarried): ");
            var maritalStatus = char.MinValue;
            var married = Console.ReadLine();
            if (married.Length > 0)
            {
                maritalStatus = married[informationAboutMaritalStatus];
            }

            Console.WriteLine($"Height (more than {MinimumHeight}): ");
            var height = short.Parse(Console.ReadLine(), CultureEnUS);

            return new RecordParameters(firstName, lastName, dateOfBirth, wallet, maritalStatus, height);
        }

        /// <inheritdoc/>
        protected override void ValidateParameters(RecordParameters recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (recordParameters.FirstName == null)
            {
                throw new ArgumentNullException(nameof(recordParameters), "The first name cannot be null.");
            }

            if (recordParameters.FirstName.Length < MinimumLengthOfFirstAndLastName || recordParameters.FirstName.Length > MaximumLengthOfFirstAndLastName || string.IsNullOrWhiteSpace(recordParameters.FirstName))
            {
                throw new ArgumentException($"The minimum length of the first name is {MinimumLengthOfFirstAndLastName}, the maximum is {MaximumLengthOfFirstAndLastName}.", nameof(recordParameters));
            }

            if (recordParameters.LastName == null)
            {
                throw new ArgumentNullException(nameof(recordParameters), "The last name cannot be null.");
            }

            if (recordParameters.LastName.Length < MinimumLengthOfFirstAndLastName || recordParameters.LastName.Length > MaximumLengthOfFirstAndLastName || string.IsNullOrWhiteSpace(recordParameters.LastName))
            {
                throw new ArgumentException($"The minimum length of the last name is {MinimumLengthOfFirstAndLastName}, the maximum is {MaximumLengthOfFirstAndLastName}.", nameof(recordParameters));
            }

            if (recordParameters.DateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(recordParameters), $"The {nameof(recordParameters)} cannot be null.");
            }

            if (recordParameters.DateOfBirth < MinimalDateOfBirth || recordParameters.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException($"Date of birth should be later than {MinimalDateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, but earlier than now.", nameof(recordParameters));
            }

            if (recordParameters.Wallet < MinimumAmountOfMoney)
            {
                throw new ArgumentException($"Money in the wallet cannot be less than {MinimumAmountOfMoney}.", nameof(recordParameters));
            }

            if (recordParameters.MaritalStatus != 'M' && recordParameters.MaritalStatus != 'm' && recordParameters.MaritalStatus != 'U' && recordParameters.MaritalStatus != 'u')
            {
                throw new ArgumentException("Marital status may be M - married, or U - unmarried.", nameof(recordParameters));
            }

            if (recordParameters.Height < MinimumHeight)
            {
                throw new ArgumentException($"Growth cannot be less than {MinimumHeight}.", nameof(recordParameters));
            }
        }
    }
}
