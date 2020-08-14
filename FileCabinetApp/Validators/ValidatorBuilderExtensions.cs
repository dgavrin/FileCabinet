using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Provides extension methods for ValidatorBuilder.
    /// </summary>
    public static class ValidatorBuilderExtensions
    {
        /// <summary>
        /// Extension method for create default record validator.
        /// </summary>
        /// <param name="validatorBuilder">Validator builder.</param>
        /// <returns>Default record validator.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder) => CreateValidator("default");

        /// <summary>
        /// Extension method for create custom record validator.
        /// </summary>
        /// <param name="validatorBuilder">Validator builder.</param>
        /// <returns>Custom record validator.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder) => CreateValidator("custom");

        private static IRecordValidator CreateValidator(string validationType)
        {
            if (string.IsNullOrEmpty(validationType))
            {
                throw new ArgumentNullException(nameof(validationType));
            }

            var configuration = new ValidationRulesConfigurationReader(validationType);
            (var minLenghtOfFirstName, var maxLengthOfFirstName) = configuration.ReadFirstNameValidationCriteria();
            (var minLenghtOfLastName, var maxLengthOfLastName) = configuration.ReadLastNameValidationCriteria();
            (var minDateOfBirth, var maxDateOfBirth) = configuration.ReadDateOfBirthValidationCriteria();
            var minAmountOfWallet = configuration.ReadWalletValidationCriteria();
            var minHeight = configuration.ReadHeightValidationCriteria();

            return new ValidatorBuilder()
                .ValidateFirstName(minLenghtOfFirstName, maxLengthOfFirstName)
                .ValidateLastName(minLenghtOfLastName, maxLengthOfLastName)
                .ValidateDateOfBirth(minDateOfBirth, maxDateOfBirth)
                .ValidateWallet(minAmountOfWallet)
                .ValidateMaritalStatus()
                .ValidateHeight(minHeight)
                .Create();
        }
    }
}
