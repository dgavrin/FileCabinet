using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for last name.
    /// </summary>
    public class DefaultLastNameValidator : IRecordValidator
    {
        private const int MinimumLengthOfLastName = 2;
        private const int MaximumLengthOfLastName = 60;

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (string.IsNullOrWhiteSpace(recordParameters.LastName) ||
                recordParameters.LastName.Length < MinimumLengthOfLastName ||
                recordParameters.LastName.Length > MaximumLengthOfLastName)
            {
                throw new ArgumentException(nameof(recordParameters.LastName), $"The last name cannot be shorter than {MinimumLengthOfLastName} and longer than {MaximumLengthOfLastName} characters.");
            }
        }
    }
}
