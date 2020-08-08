using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for first name.
    /// </summary>
    public class CustomHeightValidator : IRecordValidator
    {
        private const short MinimumHeight = 50;

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (recordParameters.Height < MinimumHeight)
            {
                throw new ArgumentException(nameof(recordParameters.Height), $"Height cannot be less than {MinimumHeight}.");
            }
        }
    }
}
