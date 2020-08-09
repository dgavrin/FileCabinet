using System;
using System.Globalization;
using System.IO;
using FileCabinetApp.Records;

namespace FileCabinetApp.Writers
{
    /// <summary>
    /// File cabinet record csv writer.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The textWriter.</param>
        public FileCabinetRecordCsvWriter(TextWriter textWriter)
        {
            this.writer = textWriter;
        }

        /// <summary>
        /// Writes a record to a csv file.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.writer.WriteLine(
                $"{record.Id}, " +
                $"{record.FirstName}, " +
                $"{record.LastName}, " +
                $"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, " +
                $"{record.Wallet}, " +
                $"{record.MaritalStatus.ToString(CultureInfo.InvariantCulture).ToUpperInvariant()}, " +
                $"{record.Height}");
        }
    }
}
