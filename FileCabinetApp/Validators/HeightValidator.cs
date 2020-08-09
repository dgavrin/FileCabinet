using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for first name.
    /// </summary>
    public class HeightValidator : IRecordValidator
    {
        private short minimumHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightValidator"/> class.
        /// </summary>
        /// <param name="minHeight">Minimum height.</param>
        public HeightValidator(short minHeight)
        {
            this.minimumHeight = minHeight;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (recordParameters.Height < this.minimumHeight)
            {
                throw new ArgumentException(nameof(recordParameters.Height), $"Height cannot be less than {this.minimumHeight}.");
            }
        }
    }
}
