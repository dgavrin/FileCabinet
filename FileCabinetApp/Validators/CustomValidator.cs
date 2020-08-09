using System;
using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents custom validator for file cabinet service.
    /// </summary>
    public class CustomValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        public CustomValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(4, 20),
                new LastNameValidator(4, 20),
                new DateOfBirthValidator(new DateTime(1990, 1, 1), DateTime.Today),
                new WalletValidator(100M),
                new MaritalStatusValidator(),
                new HeightValidator(50),
            })
        {
        }
    }
}
