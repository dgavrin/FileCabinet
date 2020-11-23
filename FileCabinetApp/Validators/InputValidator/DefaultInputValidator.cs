using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators.InputValidator
{
    /// <summary>
    /// Represents defualt validator for input.
    /// </summary>
    public class DefaultInputValidator : IInputValidator
    {
        private readonly short minimalHeight;
        private readonly int minimalLengthOfFirstName;
        private readonly int maximumLengthOfFirstName;
        private readonly int minimalLengthOfLastName;
        private readonly int maximumLengthOfLastName;
        private readonly decimal minimalAmountOfMoney;
        private readonly DateTime minimalDateOfBirth;
        private readonly DateTime maximumDateOfBirth;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultInputValidator"/> class.
        /// </summary>
        public DefaultInputValidator()
        {
            var configuration = new ValidationRulesConfigurationReader("default");

            (var minLenghtOfFirstName, var maxLengthOfFirstName) = configuration.ReadFirstNameValidationCriteria();
            this.minimalLengthOfFirstName = minLenghtOfFirstName;
            this.maximumLengthOfFirstName = maxLengthOfFirstName;

            (var minLenghtOfLastName, var maxLengthOfLastName) = configuration.ReadLastNameValidationCriteria();
            this.minimalLengthOfLastName = minLenghtOfFirstName;
            this.maximumLengthOfLastName = maxLengthOfFirstName;

            (var minDateOfBirth, var maxDateOfBirth) = configuration.ReadDateOfBirthValidationCriteria();
            this.minimalDateOfBirth = minDateOfBirth;
            this.maximumDateOfBirth = maxDateOfBirth;

            this.minimalAmountOfMoney = configuration.ReadWalletValidationCriteria();
            this.minimalHeight = configuration.ReadHeightValidationCriteria();
        }

        /// <inheritdoc/>
        public Tuple<bool, string> IdentifierValidator(int identifier)
        {
            if (identifier < 0)
            {
                return new Tuple<bool, string>(false, "identifier");
            }
            else
            {
                return new Tuple<bool, string>(true, "identifier");
            }
        }

        /// <inheritdoc/>
        public Tuple<bool, string> FirstNameValidator(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(firstName) ||
                firstName.Length < this.minimalLengthOfFirstName ||
                firstName.Length > this.maximumLengthOfFirstName)
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

            if (string.IsNullOrWhiteSpace(lastName) ||
                lastName.Length < this.minimalLengthOfLastName ||
                lastName.Length > this.maximumLengthOfLastName)
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

            if (dateOfBirth < this.minimalDateOfBirth || dateOfBirth > this.maximumDateOfBirth)
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
            if (wallet < this.minimalAmountOfMoney)
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
            if (maritalStatus != 'M' && maritalStatus != 'm' &&
                maritalStatus != 'U' && maritalStatus != 'u')
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
            if (height < this.minimalHeight)
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

            var validationResults = new List<Tuple<bool, string>>();

            validationResults.Add(this.IdentifierValidator(recordParameters.Id));
            validationResults.Add(this.FirstNameValidator(recordParameters.FirstName));
            validationResults.Add(this.LastNameValidator(recordParameters.LastName));
            validationResults.Add(this.DateOfBirthValidator(recordParameters.DateOfBirth));
            validationResults.Add(this.WalletValidator(recordParameters.Wallet));
            validationResults.Add(this.MaritalStatusValidator(recordParameters.MaritalStatus));
            validationResults.Add(this.HeightValidator(recordParameters.Height));

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
