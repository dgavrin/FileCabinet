using System;
using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.Printers
{
    /// <summary>
    /// Provides a printer for displaying records.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Prints the specified records.
        /// </summary>
        /// <param name="records">Records.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}
