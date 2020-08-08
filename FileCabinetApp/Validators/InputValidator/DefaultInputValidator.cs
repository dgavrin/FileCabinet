using System;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators.InputValidator
{
    /// <summary>
    /// Represents defualt validator for input.
    /// </summary>
    public class DefaultInputValidator : IInputValidator
    {
        private const short MinimumHeight = 0;
        private const int MinimumLengthOfFirstAndLastName = 2;
        private const int MaximumLengthOfFirstAndLastName = 60;
        private const decimal MinimumAmountOfMoney = 0M;
        private static readonly DateTime MinimalDateOfBirth = new DateTime(1950, 1, 1);

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
        public Tuple<bool, string> ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            var validationResults = new Tuple<bool, string>[6];

            validationResults[0] = this.FirstNameValidator(recordParameters.FirstName);
            validationResults[1] = this.LastNameValidator(recordParameters.LastName);
            validationResults[2] = this.DateOfBirthValidator(recordParameters.DateOfBirth);
            validationResults[3] = this.WalletValidator(recordParameters.Wallet);
            validationResults[4] = this.MaritalStatusValidator(recordParameters.MaritalStatus);
            validationResults[5] = this.HeightValidator(recordParameters.Height);

            bool validationResult = true;
            StringBuilder errorMessages = new StringBuilder();
            foreach (var result in validationResults)
            {
                validationResult = validationResult && result.Item1;
                if (!result.Item1)
                {
                    errorMessages.Append(" " + result.Item2);
                }
            }

            return new Tuple<bool, string>(validationResult, errorMessages.ToString());
        }
    }
}
