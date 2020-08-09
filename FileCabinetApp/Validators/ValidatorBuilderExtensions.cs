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
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            return new ValidatorBuilder()
                .ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Today)
                .ValidateWallet(0M)
                .ValidateMaritalStatus()
                .ValidateHeight(0)
                .Create();
        }

        /// <summary>
        /// Extension method for create custom record validator.
        /// </summary>
        /// <param name="validatorBuilder">Validator builder.</param>
        /// <returns>Custom record validator.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            return new ValidatorBuilder()
                .ValidateFirstName(4, 20)
                .ValidateLastName(4, 20)
                .ValidateDateOfBirth(new DateTime(1990, 1, 1), DateTime.Today)
                .ValidateWallet(100M)
                .ValidateMaritalStatus()
                .ValidateHeight(50)
                .Create();
        }
    }
}
