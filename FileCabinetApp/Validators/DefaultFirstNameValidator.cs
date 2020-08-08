using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for first name.
    /// </summary>
    public class DefaultFirstNameValidator : IRecordValidator
    {
        private const int MinimumLengthOfFirstName = 2;
        private const int MaximumLengthOfFirstName = 60;

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (string.IsNullOrWhiteSpace(recordParameters.FirstName) ||
                recordParameters.FirstName.Length < MinimumLengthOfFirstName ||
                recordParameters.FirstName.Length > MaximumLengthOfFirstName)
            {
                throw new ArgumentException(nameof(recordParameters.FirstName), $"The first name cannot be shorter than {MinimumLengthOfFirstName} and longer than {MaximumLengthOfFirstName} characters.");
            }
        }
    }
}
