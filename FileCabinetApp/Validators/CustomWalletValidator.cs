using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for wallet.
    /// </summary>
    public class CustomWalletValidator : IRecordValidator
    {
        private const decimal MinimumAmountOfMoney = 100M;

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (recordParameters.Wallet < MinimumAmountOfMoney)
            {
                throw new ArgumentException(nameof(recordParameters.Wallet), $"The wallet state cannot be less than {MinimumAmountOfMoney}.");
            }
        }
    }
}
