using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for first name.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private int minimumLengthOfFirstName = 4;
        private int maximumLengthOfFirstName = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Minimum length of first name.</param>
        /// <param name="maxLength">Maximum length of first name.</param>
        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minimumLengthOfFirstName = minLength;
            this.maximumLengthOfFirstName = maxLength;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (string.IsNullOrWhiteSpace(recordParameters.FirstName) ||
                recordParameters.FirstName.Length < this.minimumLengthOfFirstName ||
                recordParameters.FirstName.Length > this.maximumLengthOfFirstName)
            {
                throw new ArgumentException(nameof(recordParameters.FirstName), $"The first name cannot be shorter than {this.minimumLengthOfFirstName} and longer than {this.maximumLengthOfFirstName} characters.");
            }
        }
    }
}
