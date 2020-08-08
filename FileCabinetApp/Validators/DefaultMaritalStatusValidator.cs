using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for marital status.
    /// </summary>
    public class DefaultMaritalStatusValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (recordParameters.MaritalStatus != 'M' && recordParameters.MaritalStatus != 'm' && recordParameters.MaritalStatus != 'U' && recordParameters.MaritalStatus != 'u')
            {
                throw new ArgumentException(nameof(recordParameters.MaritalStatus), $"Marital status can be 'M' - married or 'U' - unmarried.");
            }
        }
    }
}
