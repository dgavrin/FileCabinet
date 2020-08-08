using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for date of birth.
    /// </summary>
    public class CustomDateOfBirthValidator : IRecordValidator
    {
        private static readonly DateTime MinimalDateOfBirth = new DateTime(1990, 1, 1);
        private static readonly DateTime MaximumDateOfBirth = DateTime.Now;

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (recordParameters.DateOfBirth < MinimalDateOfBirth ||
                recordParameters.DateOfBirth > MaximumDateOfBirth)
            {
                throw new ArgumentException(nameof(recordParameters.DateOfBirth), $"The date of birth cannot be earlier than {MinimalDateOfBirth} or later than {MaximumDateOfBirth}.");
            }
        }
    }
}
