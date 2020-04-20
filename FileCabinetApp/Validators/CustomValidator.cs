﻿using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents custom validator for file cabinet service.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        private const short MinimumHeight = 50;
        private const int MinimumLengthOfFirstAndLastName = 4;
        private const int MaximumLengthOfFirstAndLastName = 20;
        private const decimal MinimumAmountOfMoney = 100M;
        private static readonly DateTime MinimalDateOfBirth = new DateTime(1990, 1, 1);

        /// <inheritdoc/>
        public Tuple<bool, string> FirstNameValidator(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < MinimumLengthOfFirstAndLastName || firstName.Length > MaximumLengthOfFirstAndLastName)
            {
                return new Tuple<bool, string>(false, "first name");
            }
            else
            {
                return new Tuple<bool, string>(true, "first name");
            }
        }

        /// <inheritdoc/>
        public Tuple<bool, string> LastNameValidator(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < MinimumLengthOfFirstAndLastName || lastName.Length > MaximumLengthOfFirstAndLastName)
            {
                return new Tuple<bool, string>(false, "last name");
            }
            else
            {
                return new Tuple<bool, string>(true, "last name");
            }
        }

        /// <inheritdoc/>
        public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            if (dateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(dateOfBirth), $"The date of birth cannot be null.");
            }

            if (dateOfBirth < MinimalDateOfBirth || dateOfBirth > DateTime.Now)
            {
                return new Tuple<bool, string>(false, "date of birth");
            }
            else
            {
                return new Tuple<bool, string>(true, "date of birth");
            }
        }

        /// <inheritdoc/>
        public Tuple<bool, string> WalletValidator(decimal wallet)
        {
            if (wallet < MinimumAmountOfMoney)
            {
                return new Tuple<bool, string>(false, "wallet");
            }
            else
            {
                return new Tuple<bool, string>(true, "wallet");
            }
        }

        /// <inheritdoc/>
        public Tuple<bool, string> MaritalStatusValidator(char maritalStatus)
        {
            if (maritalStatus != 'M' && maritalStatus != 'm' && maritalStatus != 'U' && maritalStatus != 'u')
            {
                return new Tuple<bool, string>(false, "marital status");
            }
            else
            {
                return new Tuple<bool, string>(true, "marital status");
            }
        }

        /// <inheritdoc/>
        public Tuple<bool, string> HeightValidator(short height)
        {
            if (height < MinimumHeight)
            {
                return new Tuple<bool, string>(false, "height");
            }
            else
            {
                return new Tuple<bool, string>(true, "height");
            }
        }

        /// <inheritdoc/>
        public void ValidateParameters(RecordParameters recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            this.FirstNameValidator(recordParameters.FirstName);
            this.LastNameValidator(recordParameters.LastName);
            this.DateOfBirthValidator(recordParameters.DateOfBirth);
            this.WalletValidator(recordParameters.Wallet);
            this.MaritalStatusValidator(recordParameters.MaritalStatus);
            this.HeightValidator(recordParameters.Height);
        }
    }
}
