using System;
using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.Printers
{
    /// <summary>
    /// Standard record printer.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <inheritdoc/>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }
    }
}
