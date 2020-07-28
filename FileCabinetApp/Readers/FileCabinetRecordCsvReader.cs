using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Records;

namespace FileCabinetApp.Readers
{
    /// <summary>
    /// File cabinet records csv reader.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="streamReader">StreamReader.</param>
        public FileCabinetRecordCsvReader(StreamReader streamReader)
        {
            this.reader = streamReader;
        }

        /// <summary>
        /// Gets a list of records to import.
        /// </summary>
        /// <returns>List of records to import.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> readRecords = new List<FileCabinetRecord>();

            _ = this.reader.ReadLine();
            while (!this.reader.EndOfStream)
            {
                var nextRecord = this.reader.ReadLine();
                var recordFields = nextRecord.Split(", ");

                int nextRecordId;
                DateTime nextRecordDateOfBirth;
                decimal nextRecordWallet;
                char nextRecordMaritalStatus;
                short nextRecordHeight;

                if (!int.TryParse(recordFields[0], out nextRecordId))
                {
                    continue;
                }

                if (!DateTime.TryParse(recordFields[3], out nextRecordDateOfBirth))
                {
                    continue;
                }

                if (!decimal.TryParse(recordFields[4], out nextRecordWallet))
                {
                    continue;
                }

                if (!char.TryParse(recordFields[5], out nextRecordMaritalStatus))
                {
                    continue;
                }

                if (!short.TryParse(recordFields[6], out nextRecordHeight))
                {
                    continue;
                }

                var record = new FileCabinetRecord
                {
                    Id = nextRecordId,
                    FirstName = recordFields[1],
                    LastName = recordFields[2],
                    DateOfBirth = nextRecordDateOfBirth,
                    Wallet = nextRecordWallet,
                    MaritalStatus = nextRecordMaritalStatus,
                    Height = nextRecordHeight,
                };

                readRecords.Add(record);
            }

            return readRecords;
        }
    }
}
