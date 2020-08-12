using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Gets information about validation rules from json configuration file.
    /// </summary>
    public class ValidationRulesConfigurationReader
    {
        private IConfiguration config;
        private string validationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRulesConfigurationReader"/> class.
        /// </summary>
        /// <param name="validationType">Validation type.</param>
        public ValidationRulesConfigurationReader(string validationType)
        {
            if (string.IsNullOrEmpty(validationType))
            {
                throw new ArgumentNullException(nameof(validationType));
            }

            if (validationType.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
            {
                this.validationType = "custom";
            }
            else
            {
                this.validationType = "default";
            }

            this.config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("validation-rules.json")
                .Build();
        }

        /// <summary>
        /// Returns the criteria for checking the first name.
        /// </summary>
        /// <returns>A tuple of minimum and maximum first name lengths.</returns>
        public Tuple<int, int> ReadFirstNameValidationCriteria()
        {
            var firstNameSection = this.config.GetSection(this.validationType).GetSection("firstName");
            return Tuple.Create(firstNameSection.GetValue<int>("min"), firstNameSection.GetValue<int>("max"));
        }

        /// <summary>
        /// Returns the criteria for checking the last name.
        /// </summary>
        /// <returns>A tuple of minimum and maximum last name lengths.</returns>
        public Tuple<int, int> ReadLastNameValidationCriteria()
        {
            var lastNameSection = this.config.GetSection(this.validationType).GetSection("lastName");
            return Tuple.Create(lastNameSection.GetValue<int>("min"), lastNameSection.GetValue<int>("max"));
        }

        /// <summary>
        /// Returns the criteria for checking the date of birth.
        /// </summary>
        /// <returns>A tuple of the minimum and maximum birthday dates.</returns>
        public Tuple<DateTime, DateTime> ReadDateOfBirthValidationCriteria()
        {
            var dateOfBirthSection = this.config.GetSection(this.validationType).GetSection("dateOfBirth");
            return Tuple.Create(dateOfBirthSection.GetValue<DateTime>("from"), dateOfBirthSection.GetValue<DateTime>("to"));
        }

        /// <summary>
        /// Returns the criteria for checking the wallet.
        /// </summary>
        /// <returns>Minimum wallet amount.</returns>
        public decimal ReadWalletValidationCriteria()
        {
            return this.config.GetSection(this.validationType).GetSection("wallet").GetValue<decimal>("min");
        }

        /// <summary>
        /// Returns the criteria for checking the height.
        /// </summary>
        /// <returns>Minimum height.</returns>
        public short ReadHeightValidationCriteria()
        {
            return this.config.GetSection(this.validationType).GetSection("height").GetValue<short>("min");
        }
    }
}
