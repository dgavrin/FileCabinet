using System;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents an object that stores parameters for creating and editing records in a file cabinet.
    /// </summary>
    public class RecordParameters : FileCabinetRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordParameters"/> class.
        /// </summary>
        public RecordParameters()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordParameters"/> class.
        /// </summary>
        /// <param name="firstName"> The first name. </param>
        /// <param name="lastName"> The last name. </param>
        /// <param name="dateOfBirth"> The date of birth. </param>
        /// <param name="wallet"> The wallet. </param>
        /// <param name="maritalStatus"> The marital status. </param>
        /// <param name="height"> The height. </param>
        public RecordParameters(string firstName, string lastName, DateTime dateOfBirth, decimal wallet, char maritalStatus, short height)
            : base()
        {
            RecordValidator.FirstAndLastNameVallidator(firstName);
            RecordValidator.FirstAndLastNameVallidator(lastName);
            RecordValidator.DateOfBirthValidator(dateOfBirth);
            RecordValidator.WalletValidator(wallet);
            RecordValidator.MaritalStatusValidator(maritalStatus);
            RecordValidator.HeightValidator(height);

            this.Id = 0;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Wallet = wallet;
            this.MaritalStatus = maritalStatus;
            this.Height = height;
        }
    }
}
