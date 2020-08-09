using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Record validator.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="recordParameters"> Record fileds. </param>
        public void ValidateParameters(FileCabinetRecord recordParameters);
    }
}
