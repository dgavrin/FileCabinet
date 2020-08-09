using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Composite validator.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Validators.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators.ToList<IRecordValidator>() ?? throw new ArgumentNullException(nameof(validators));
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(recordParameters);
            }
        }
    }
}
