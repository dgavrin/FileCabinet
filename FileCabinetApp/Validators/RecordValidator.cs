using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents input-validating methods for records.
    /// </summary>
    public static class RecordValidator
    {
        private const short MinimumHeight = 0;
        private const int MinimumLengthOfFirstAndLastName = 2;
        private const int MaximumLengthOfFirstAndLastName = 60;
        private const decimal MinimumAmountOfMoney = 0M;
        private static readonly DateTime MinimalDateOfBirth = new DateTime(1950, 1, 1);

        /// <summary>
        /// Throws an exception if the first or last name is incorrect.
        /// </summary>
        /// <param name="name"> First or last name. </param>
        public static void FirstAndLastNameVallidator(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), $"The {nameof(name)} cannot be null.");
            }

            if (name.Length < MinimumLengthOfFirstAndLastName || name.Length > MaximumLengthOfFirstAndLastName || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The minimum length of the first name is 2, the maximum is 60.", nameof(name));
            }
        }

        /// <summary>
        /// Throws an exception if the date of birth is incorrect.
        /// </summary>
        /// <param name="dateOfBirth"> The date of birth. </param>
        public static void DateOfBirthValidator(DateTime dateOfBirth)
        {
            if (dateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(dateOfBirth), $"The {nameof(dateOfBirth)} cannot be null.");
            }

            if (dateOfBirth < MinimalDateOfBirth || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth should be later than January 1, 1950, but earlier than now.", nameof(dateOfBirth));
            }
        }

        /// <summary>
        /// Throws an exception if the wallet amount is incorrect.
        /// </summary>
        /// <param name="wallet"> The wallet. </param>
        public static void WalletValidator(decimal wallet)
        {
            if (wallet < MinimumAmountOfMoney)
            {
                throw new ArgumentException("Money in the wallet cannot be less than zero.", nameof(wallet));
            }
        }

        /// <summary>
        /// Throws an exception if the marital status is incorrect.
        /// </summary>
        /// <param name="maritalStatus"> The marital status. </param>
        public static void MaritalStatusValidator(char maritalStatus)
        {
            if (maritalStatus != 'M' && maritalStatus != 'm' && maritalStatus != 'U' && maritalStatus != 'u')
            {
                throw new ArgumentException("Marital status may be M - married, or U - unmarried.", nameof(maritalStatus));
            }
        }

        /// <summary>
        /// Throws an exception if the height value is incorrect.
        /// </summary>
        /// <param name="height"> The height. </param>
        public static void HeightValidator(short height)
        {
            if (height < MinimumHeight)
            {
                throw new ArgumentException("Growth cannot be less than zero.", nameof(height));
            }
        }
    }
}
