using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validator builder.
    /// </summary>
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Builds a first name validator.
        /// </summary>
        /// <param name="minLength">Minimum length.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <returns>This instance.</returns>
        public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Builds a last name validator.
        /// </summary>
        /// <param name="minLength">Minimum length.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <returns>This instance.</returns>
        public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Builds a date of birth validator.
        /// </summary>
        /// <param name="from">Minimum date of birth.</param>
        /// <param name="to">Maximum date of birth.</param>
        /// <returns>This instance.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Builds a wallet validator.
        /// </summary>
        /// <param name="minAmountOfMoney">Minimum amount of money.</param>
        /// <returns>This instance.</returns>
        public ValidatorBuilder ValidateWallet(decimal minAmountOfMoney)
        {
            this.validators.Add(new WalletValidator(minAmountOfMoney));
            return this;
        }

        /// <summary>
        /// Builds a marital status validator.
        /// </summary>
        /// <returns>This instance.</returns>
        public ValidatorBuilder ValidateMaritalStatus()
        {
            this.validators.Add(new MaritalStatusValidator());
            return this;
        }

        /// <summary>
        /// Builds a height validator.
        /// </summary>
        /// <param name="minHeight">Minimum height.</param>
        /// <returns>This. instance.</returns>
        public ValidatorBuilder ValidateHeight(short minHeight)
        {
            this.validators.Add(new HeightValidator(minHeight));
            return this;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>IRecordValidator.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
