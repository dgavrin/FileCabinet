using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using FileCabinetApp.Records;
using FileCabinetApp.Validators;

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
        }

        /// <inheritdoc/>
        public void DisplayRecords(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                var dateOfBirth = record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));

                var maritalStatus = "unmarried";
                if (record.MaritalStatus == 'M' || record.MaritalStatus == 'm')
                {
                    maritalStatus = "married";
                }

                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {dateOfBirth}, {record.Wallet}$, {maritalStatus}, {record.Height}cm");
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
                Id = (int)(this.fileStream.Length / RecordSize) + 1,
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
            var recordBuffer = new byte[RecordSize];

            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0, recordNumber = 0; i < this.fileStream.Length; i += RecordSize, recordNumber++)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                var temporaryRecord = BytesToFileCabinetRecord(recordBuffer);

                if (temporaryRecord.Id == id)
                {
                    recordNumberToChange = recordNumber;
                    break;
                }
            }

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
                records.Add(BytesToFileCabinetRecord(recordBuffer));
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return (int)(this.fileStream.Length / RecordSize);
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public RecordParameters SetInformationToRecord()
        {
            Console.Write("First Name: ");
            var firstName = ReadInput(InputConverters.StringConverter, this.validator.FirstNameValidator);

            Console.Write("Last Name: ");
            var lastName = ReadInput(InputConverters.StringConverter, this.validator.LastNameValidator);

            Console.Write("Date of birth (MM/DD/YYYY): ");
            var dateOfBirth = ReadInput(InputConverters.DateConverter, this.validator.DateOfBirthValidator);

            Console.Write("Wallet: ");
            var wallet = ReadInput(InputConverters.WalletConverter, this.validator.WalletValidator);

            Console.Write("Marital status ('M' - married, 'U' - unmarried): ");
            var maritalStatus = ReadInput(InputConverters.MaritalStatusConverter, this.validator.MaritalStatusValidator);

            Console.Write("Height: ");
            var height = ReadInput(InputConverters.HeightConverter, this.validator.HeightValidator);

            return new RecordParameters(firstName, lastName, dateOfBirth, wallet, maritalStatus, height);
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
            throw new NotImplementedException();
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

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
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

        private static FileCabinetRecord BytesToFileCabinetRecord(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length < RecordSize)
            {
                throw new ArgumentException("Error. Record is corrupted.", nameof(bytes));
            }

            var record = new FileCabinetRecord();

            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                short status = binaryReader.ReadInt16();
                record.Id = binaryReader.ReadInt32();

                record.FirstName = binaryReader.ReadString().Trim(' ');
                record.LastName = binaryReader.ReadString().Trim(' ');

                record.DateOfBirth = new DateTime(
                    binaryReader.ReadInt32(),
                    binaryReader.ReadInt32(),
                    binaryReader.ReadInt32());

                record.Wallet = binaryReader.ReadDecimal();

                record.MaritalStatus = binaryReader.ReadChar();

                record.Height = binaryReader.ReadInt16();
            }

            return record;
        }
    }
}
