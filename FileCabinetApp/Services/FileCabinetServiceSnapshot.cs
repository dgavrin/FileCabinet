using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Records;
using FileCabinetApp.Writers;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Represents a snapshot of a file cabinet service.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="list"> List of records. </param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            this.records = list.ToArray();
        }

        /// <summary>
        /// Saves a list of entries to a csv file.
        /// </summary>
        /// <param name="streamWriter"> Stream for recording. </param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            if (streamWriter == null)
            {
                throw new ArgumentNullException(nameof(streamWriter));
            }

            var csvWriter = new FileCabinetRecordCsvWriter(streamWriter);
            streamWriter.WriteLine("Id, First name, Last name, Date of Birth, Wallet, Marital status, Height");

            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }
    }
}
