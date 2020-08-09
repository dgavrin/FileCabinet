using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for file cabinet service.
    /// </summary>
    public class DefaultValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        public DefaultValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(2, 60),
                new LastNameValidator(2, 60),
                new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Today),
                new WalletValidator(0M),
                new MaritalStatusValidator(),
                new HeightValidator(0),
            })
        {
        }
    }
}
