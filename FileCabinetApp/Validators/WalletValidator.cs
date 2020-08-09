using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents defualt validator for wallet.
    /// </summary>
    public class WalletValidator : IRecordValidator
    {
        private decimal minimumAmountOfMoney = 100M;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletValidator"/> class.
        /// </summary>
        /// <param name="minAmountOfMoney">Minimum amount of money.</param>
        public WalletValidator(decimal minAmountOfMoney)
        {
            this.minimumAmountOfMoney = minAmountOfMoney;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (recordParameters.Wallet < this.minimumAmountOfMoney)
            {
                throw new ArgumentException(nameof(recordParameters.Wallet), $"The wallet state cannot be less than {this.minimumAmountOfMoney}.");
            }
        }
    }
}
