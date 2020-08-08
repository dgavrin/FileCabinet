using System;
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

            new FirstNameValidator(4, 20).ValidateParameters(recordParameters);
            new LastNameValidator(4, 20).ValidateParameters(recordParameters);
            new DateOfBirthValidator(new DateTime(1990, 1, 1), DateTime.Now).ValidateParameters(recordParameters);
            new WalletValidator(100M).ValidateParameters(recordParameters);
            new MaritalStatusValidator().ValidateParameters(recordParameters);
            new HeightValidator(50).ValidateParameters(recordParameters);
        }
    }
}
