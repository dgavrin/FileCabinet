using System;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents custom validator for file cabinet service.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            new CustomFirstNameValidator().ValidateParameters(recordParameters);
            new CustomLastNameValidator().ValidateParameters(recordParameters);
            new CustomDateOfBirthValidator().ValidateParameters(recordParameters);
            new CustomWalletValidator().ValidateParameters(recordParameters);
            new CustomMaritalStatusValidator().ValidateParameters(recordParameters);
            new CustomHeightValidator().ValidateParameters(recordParameters);
        }
    }
}
