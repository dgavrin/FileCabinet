using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using FileCabinetApp.Records;
using FileCabinetApp.Validators;
using FileCabinetApp.Validators.InputValidator;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// It is an abstract service in which records with personal information about a person in the file system are stored.
    /// </summary>
    public class FileCabinetFileSystemService : IFileCabinetService, IDisposable
    {
        private const int MaximumLengthOfFirstAndLastName = 60;
        private const int RecordSize = sizeof(short) // Status
                                     + sizeof(int) // Id
                                     + (4 * MaximumLengthOfFirstAndLastName) // FirstName and LastName
                                     + sizeof(int) // DateOfBirth: Year
                                     + sizeof(int) // DateOfBirth: Month
                                     + sizeof(int) // DateOfBirth: Day
                                     + sizeof(decimal) // Wallet
                                     + sizeof(char) // MaritalStatus
                                     + sizeof(short); // Height

        private readonly FileStream fileStream;
        private readonly BinaryWriter binaryWriter;

        private IRecordValidator validator;
        private bool disposedValue;
        private int lastRecordId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFileSystemService"/> class.
        /// </summary>
        /// <param name="fileStream"> File stream. </param>
        /// <param name="validationType"> The validation type. </param>
        public FileCabinetFileSystemService(FileStream fileStream, string validationType)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }
            else
            {
                this.fileStream = fileStream;
                this.binaryWriter = new BinaryWriter(this.fileStream);
            }

            if (validationType == null)
            {
                throw new ArgumentNullException(nameof(validationType));
            }

            if (validationType.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
            {
                this.validator = new CustomValidator();
            }
            else
            {
                this.validator = new DefaulValidator();
            }

            this.lastRecordId = this.GetLastRecordId();
        }

        /// <inheritdoc/>
        public IInputValidator InputValidator
        {
            get
            {
                if (this.validator is CustomValidator)
                {
                    return new CustomInputValidator();
                }
                else
                {
                    return new DefaulInputValidator();
                }
            }
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameters recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            this.validator.ValidateParameters(recordParameters);

            var newRecord = new FileCabinetRecord
            {
                Id = ++this.lastRecordId,
                FirstName = recordParameters.FirstName,
                LastName = recordParameters.LastName,
                DateOfBirth = recordParameters.DateOfBirth,
                Wallet = recordParameters.Wallet,
                MaritalStatus = recordParameters.MaritalStatus,
                Height = recordParameters.Height,
            };
            var bytesOfNewRecord = FileCabinetRecordToBytes(newRecord);
            this.fileStream.Seek(0, SeekOrigin.End);
            this.fileStream.Write(bytesOfNewRecord, 0, bytesOfNewRecord.Length);
            this.fileStream.Flush();

            return newRecord.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordParameters recordParameters)
        {
            if (id < 0)
            {
                throw new ArgumentException($"The {nameof(id)} cannot be less than zero.");
            }

            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            recordParameters.Id = id;
            this.validator.ValidateParameters(recordParameters);

            var recordNumberToChange = -1;
            this.TryGetIndexOfRecordWithId(id, out recordNumberToChange);

            if (recordNumberToChange >= 0)
            {
                var bytesOfRecordParameters = FileCabinetRecordToBytes(recordParameters);
                this.fileStream.Seek(recordNumberToChange * RecordSize, SeekOrigin.Begin);
                this.fileStream.Write(bytesOfRecordParameters, 0, bytesOfRecordParameters.Length);
                this.fileStream.Flush();
            }
            else
            {
                throw new ArgumentException($"#{id} record is not found.", nameof(id));
            }
        }

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        public void Purge()
        {
            var totalNumberOfRecords = (int)(this.fileStream.Length / RecordSize);
            var records = this.GetRecords();

            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var record in records)
            {
                var bytesOfRecord = FileCabinetRecordToBytes(record);
                this.fileStream.Write(bytesOfRecord, 0, bytesOfRecord.Length);
            }

            this.fileStream.Flush();
            this.fileStream.SetLength(this.fileStream.Position);

            Console.WriteLine($"Data file processing is completed: {totalNumberOfRecords - records.Count} of {totalNumberOfRecords} records were purged.");
            Console.WriteLine();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            if (dateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(dateOfBirth));
            }

            if (dateOfBirth.Length == 0)
            {
                throw new ArgumentException("There are no entries with an empty date of birth.", nameof(dateOfBirth));
            }

            var date = DateTime.MinValue;
            if (!DateTime.TryParse(dateOfBirth, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                throw new ArgumentException("Wrong date format.", nameof(dateOfBirth));
            }

            var foundRecords = from record in this.GetRecords()
                               where record.DateOfBirth.Equals(date)
                               select record;

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>(foundRecords));
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (firstName.Length == 0)
            {
                throw new ArgumentException($"There are no entries with an empty first name.", nameof(firstName));
            }

            var foundRecords = from record in this.GetRecords()
                               where record.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase)
                               select record;

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>(foundRecords));
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (lastName.Length == 0)
            {
                throw new ArgumentException($"There are no entries with an empty last name.", nameof(lastName));
            }

            var foundRecords = from record in this.GetRecords()
                               where record.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase)
                               select record;

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>(foundRecords));
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            var recordBuffer = new byte[RecordSize];

            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < this.fileStream.Length; i += RecordSize)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                FileCabinetRecord temporaryRecord;

                if (BytesToFileCabinetRecord(recordBuffer, out temporaryRecord))
                {
                    records.Add(temporaryRecord);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public (int active, int removed) GetStat()
        {
            var active = this.GetRecords().Count;
            var removed = (int)(this.fileStream.Length / RecordSize) - active;

            return (active, removed);
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var list = new List<FileCabinetRecord>(this.GetRecords());
            return new FileCabinetServiceSnapshot(list);
        }

        /// <summary>
        /// Releases fileStream, binaryReader, binaryWriter.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            if (fileCabinetServiceSnapshot == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetServiceSnapshot));
            }

            var loadedRecords = fileCabinetServiceSnapshot.Records;
            var importedRecordsCount = 0;

            foreach (var importedRecord in loadedRecords)
            {
                var inputValidator = this.InputValidator;
                var validationResult = inputValidator.ValidateParameters(importedRecord);
                if (validationResult.Item1)
                {
                    var importedRecordParameters = new RecordParameters(importedRecord);
                    importedRecordsCount++;
                    try
                    {
                        this.EditRecord(importedRecord.Id, importedRecordParameters);
                    }
                    catch (ArgumentException)
                    {
                        this.CreateRecord(importedRecordParameters);
                    }
                }
                else
                {
                    Console.WriteLine($"Error. Record #{importedRecord.Id} is not imported. Invalid field: {validationResult.Item2}.");
                }
            }

            return importedRecordsCount;
        }

        /// <inheritdoc/>
        public bool Remove(int recordIdForRemove)
        {
            if (recordIdForRemove < 1)
            {
                throw new ArgumentException($"The {nameof(recordIdForRemove)} cannot be less than one.");
            }

            var indexOfRecordForRemove = -1;
            if (this.TryGetIndexOfRecordWithId(recordIdForRemove, out indexOfRecordForRemove))
            {
                this.fileStream.Seek(indexOfRecordForRemove * RecordSize, SeekOrigin.Begin);
                this.fileStream.WriteByte(1);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Releases fileStream, binaryReader, binaryWriter.
        /// </summary>
        /// <param name="disposing"> Disposing. </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.fileStream.Close();
                    this.binaryWriter.Close();
                }

                this.disposedValue = true;
            }
        }

        private static byte[] FileCabinetRecordToBytes(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            var bytes = new byte[RecordSize];
            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                short status = 0;
                binaryWriter.Write(status);

                binaryWriter.Write(fileCabinetRecord.Id);

                binaryWriter.Write(fileCabinetRecord.FirstName.PadRight(MaximumLengthOfFirstAndLastName));
                binaryWriter.Write(fileCabinetRecord.LastName.PadRight(MaximumLengthOfFirstAndLastName));

                binaryWriter.Write(fileCabinetRecord.DateOfBirth.Year);
                binaryWriter.Write(fileCabinetRecord.DateOfBirth.Month);
                binaryWriter.Write(fileCabinetRecord.DateOfBirth.Day);

                binaryWriter.Write(fileCabinetRecord.Wallet);

                binaryWriter.Write(fileCabinetRecord.MaritalStatus);

                binaryWriter.Write(fileCabinetRecord.Height);
            }

            return bytes;
        }

        private static bool BytesToFileCabinetRecord(byte[] bytes, out FileCabinetRecord fileCabinetRecord)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length < RecordSize)
            {
                throw new ArgumentException("Error. Record is corrupted.", nameof(bytes));
            }

            fileCabinetRecord = new FileCabinetRecord();

            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                short status = binaryReader.ReadInt16();

                if (status == 0)
                {
                    fileCabinetRecord.Id = binaryReader.ReadInt32();

                    fileCabinetRecord.FirstName = binaryReader.ReadString().Trim(' ');
                    fileCabinetRecord.LastName = binaryReader.ReadString().Trim(' ');

                    fileCabinetRecord.DateOfBirth = new DateTime(
                        binaryReader.ReadInt32(),
                        binaryReader.ReadInt32(),
                        binaryReader.ReadInt32());

                    fileCabinetRecord.Wallet = binaryReader.ReadDecimal();

                    fileCabinetRecord.MaritalStatus = binaryReader.ReadChar();

                    fileCabinetRecord.Height = binaryReader.ReadInt16();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool TryGetIndexOfRecordWithId(int id, out int index)
        {
            index = -1;
            this.fileStream.Seek(0, SeekOrigin.Begin);

            var recordBuffer = new byte[RecordSize];

            for (int position = 0, i = 0; position < this.fileStream.Length; position += RecordSize, i++)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                FileCabinetRecord temporaryRecord;

                if (BytesToFileCabinetRecord(recordBuffer, out temporaryRecord) && temporaryRecord.Id == id)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }

        private int GetLastRecordId()
        {
            var maxRecordId = 1;
            var listOfRecords = this.GetRecords();

            foreach (var record in listOfRecords)
            {
                if (maxRecordId < record.Id)
                {
                    maxRecordId = record.Id;
                }
            }

            return maxRecordId;
        }
    }
}
