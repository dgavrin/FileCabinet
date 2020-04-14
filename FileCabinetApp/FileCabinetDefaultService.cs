using System;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents the default service, which stores records with personal information about a person.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        private const short MinimumHeight = 0;
        private const decimal MinimumAmountOfMoney = 0M;

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
        protected override IRecordValidator CreateValidator()
        {
            return new DefaulValidator();
        }
    }
}