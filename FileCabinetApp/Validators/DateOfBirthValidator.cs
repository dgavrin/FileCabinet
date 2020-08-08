using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for date of birth.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private DateTime minimalDateOfBirth;
        private DateTime maximumDateOfBirth;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Minimal date of birth.</param>
        /// <param name="to">Maximal date of birth.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.minimalDateOfBirth = from;
            this.maximumDateOfBirth = to;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (recordParameters.DateOfBirth < this.minimalDateOfBirth ||
                recordParameters.DateOfBirth > this.maximumDateOfBirth)
            {
                throw new ArgumentException(nameof(recordParameters.DateOfBirth), $"The date of birth cannot be earlier than {this.minimalDateOfBirth} or later than {this.maximumDateOfBirth}.");
            }
        }
    }
}
