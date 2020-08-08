using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for file cabinet service.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            var validationResults = new Tuple<bool, string>[6];

            new FirstNameValidator(2, 60).ValidateParameters(recordParameters);
            new LastNameValidator(2, 60).ValidateParameters(recordParameters);
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now).ValidateParameters(recordParameters);
            new WalletValidator(0M).ValidateParameters(recordParameters);
            new MaritalStatusValidator().ValidateParameters(recordParameters);
            new HeightValidator(0).ValidateParameters(recordParameters);
        }
    }
}
