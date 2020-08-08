using System;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for file cabinet service.
    /// </summary>
    public class DefaulValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            var validationResults = new Tuple<bool, string>[6];

            new DefaultFirstNameValidator().ValidateParameters(recordParameters);
            new DefaultLastNameValidator().ValidateParameters(recordParameters);
            new DefaultDateOfBirthValidator().ValidateParameters(recordParameters);
            new DefaultWalletValidator().ValidateParameters(recordParameters);
            new DefaultMaritalStatusValidator().ValidateParameters(recordParameters);
            new DefaultHeightValidator().ValidateParameters(recordParameters);
        }
    }
}
