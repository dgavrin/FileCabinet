using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for last name.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private int minimumLengthOfLastName;
        private int maximumLengthOfLastName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Minimum length of last name.</param>
        /// <param name="maxLength">Maximum length of last name.</param>
        public LastNameValidator(int minLength, int maxLength)
        {
            this.minimumLengthOfLastName = minLength;
            this.maximumLengthOfLastName = maxLength;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (string.IsNullOrWhiteSpace(recordParameters.LastName) ||
                recordParameters.LastName.Length < this.minimumLengthOfLastName ||
                recordParameters.LastName.Length > this.maximumLengthOfLastName)
            {
                throw new ArgumentException(nameof(recordParameters.LastName), $"The last name cannot be shorter than {this.minimumLengthOfLastName} and longer than {this.maximumLengthOfLastName} characters.");
            }
        }
    }
}
