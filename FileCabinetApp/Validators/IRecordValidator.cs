using System;

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
        public void ValidateParameters(RecordParameters recordParameters);

        /// <summary>
        /// Checks the validity of the first name.
        /// </summary>
        /// <param name="firstName"> The first name. </param>
        /// <returns> Validation result. </returns>
        public Tuple<bool, string> FirstNameValidator(string firstName);

        /// <summary>
        /// Checks the validity of the last name.
        /// </summary>
        /// <param name="lastName"> The last name. </param>
        /// <returns> Validation result. </returns>
        public Tuple<bool, string> LastNameValidator(string lastName);

        /// <summary>
        /// Checks the validity of the date of birth.
        /// </summary>
        /// <param name="dateOfBirth"> The date of birth. </param>
        /// <returns> Validation result. </returns>
        public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth);

        /// <summary>
        /// Checks wallet validity.
        /// </summary>
        /// <param name="wallet"> The wallet. </param>
        /// <returns> Validation result. </returns>
        public Tuple<bool, string> WalletValidator(decimal wallet);

        /// <summary>
        /// Checks the validity of marital status.
        /// </summary>
        /// <param name="maritalStatus"> The marital status. </param>
        /// <returns> Validation result. </returns>
        public Tuple<bool, string> MaritalStatusValidator(char maritalStatus);

        /// <summary>
        /// Checks the validity of height.
        /// </summary>
        /// <param name="height"> The height. </param>
        /// <returns> Validation result. </returns>
        public Tuple<bool, string> HeightValidator(short height);
    }
}
