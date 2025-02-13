﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using FileCabinetApp.Iterators;
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
        /// <summary>
        /// Record size in bytes.
        /// </summary>
        public const int RecordSize = sizeof(short) // Status
                                     + sizeof(int) // Id
                                     + (4 * MaximumLengthOfFirstAndLastName) // FirstName and LastName
                                     + sizeof(int) // DateOfBirth: Year
                                     + sizeof(int) // DateOfBirth: Month
                                     + sizeof(int) // DateOfBirth: Day
                                     + sizeof(decimal) // Wallet
                                     + sizeof(char) // MaritalStatus
                                     + sizeof(short); // Height

        private const int MaximumLengthOfFirstAndLastName = 60;
        private const string SignOR = "OR";
        private const string SignAND = "AND";

        private readonly FileStream fileStream;
        private readonly BinaryWriter binaryWriter;

        private Dictionary<int, long> identifierDictionary = new Dictionary<int, long>();
        private Dictionary<string, List<long>> firstNameDictionary = new Dictionary<string, List<long>>();
        private Dictionary<string, List<long>> lastNameDictionary = new Dictionary<string, List<long>>();
        private Dictionary<DateTime, List<long>> dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();
        private Dictionary<decimal, List<long>> walletDictionary = new Dictionary<decimal, List<long>>();
        private Dictionary<char, List<long>> maritalStatusDictionary = new Dictionary<char, List<long>>();
        private Dictionary<short, List<long>> heightDictionary = new Dictionary<short, List<long>>();

        private IRecordValidator validator;
        private bool disposedValue;
        private int lastRecordId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFileSystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="validationType">The validation type.</param>
        public FileCabinetFileSystemService(FileStream fileStream, string validationType)
        {
            if (string.IsNullOrEmpty(validationType))
            {
                throw new ArgumentNullException(nameof(validationType));
            }

            if (validationType.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
            {
                this.validator = new ValidatorBuilder().CreateCustom();
            }
            else
            {
                this.validator = new ValidatorBuilder().CreateDefault();
            }

            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.binaryWriter = new BinaryWriter(this.fileStream);

            this.UpdateDictionaries();
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
                    return new DefaultInputValidator();
                }
            }
        }

        /// <summary>
        /// Converts a set of bytes to a FileCabinetRecord and returns a value indicating whether the conversion was successful.
        /// </summary>
        /// <param name="bytes">Record in bytes.</param>
        /// <param name="fileCabinetRecord">FileCabinetRecord.</param>
        /// <returns>Successful conversion.</returns>
        public static bool BytesToFileCabinetRecord(byte[] bytes, out FileCabinetRecord fileCabinetRecord)
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

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord recordParameters, bool useId = false)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (recordParameters.Id < 0)
            {
                throw new ArgumentException("The record ID must be greater than zero.", nameof(recordParameters));
            }

            this.validator.ValidateParameters(recordParameters);

            if (!useId)
            {
                recordParameters.Id = ++this.lastRecordId;
            }

            var bytesOfNewRecord = FileCabinetRecordToBytes(recordParameters);
            this.fileStream.Seek(0, SeekOrigin.End);
            this.AddEntryToDictionaries(recordParameters, this.fileStream.Position);
            this.fileStream.Write(bytesOfNewRecord, 0, bytesOfNewRecord.Length);
            this.fileStream.Flush();
            this.UpdateLastRecordId();

            return recordParameters.Id;
        }

        /// <inheritdoc/>
        public int Insert(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            this.validator.ValidateParameters(fileCabinetRecord);

            if (fileCabinetRecord.Id > 0)
            {
                if (!this.identifierDictionary.ContainsKey(fileCabinetRecord.Id))
                {
                    return this.CreateRecord(fileCabinetRecord, true);
                }
                else
                {
                    throw new ArgumentException("A record with the given ID already exists.", nameof(fileCabinetRecord));
                }
            }
            else
            {
                return this.CreateRecord(fileCabinetRecord);
            }
        }

        /// <inheritdoc/>
        public void EditRecord(int id, FileCabinetRecord recordParameters)
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

            if (this.identifierDictionary.ContainsKey(recordParameters.Id))
            {
                var recordForRemoveFromDictionaries = new FileCabinetRecord();
                this.TryGetRecordWithId(recordParameters.Id, ref recordForRemoveFromDictionaries);
                this.RemoveEntryFromDictionaries(recordForRemoveFromDictionaries, this.identifierDictionary[recordParameters.Id]);
                var bytesOfRecordParameters = FileCabinetRecordToBytes(recordParameters);
                this.fileStream.Seek(this.identifierDictionary[recordParameters.Id], SeekOrigin.Begin);
                this.AddEntryToDictionaries(recordParameters, this.fileStream.Position);
                this.fileStream.Write(bytesOfRecordParameters, 0, bytesOfRecordParameters.Length);
                this.fileStream.Flush();
            }
            else
            {
                throw new ArgumentException($"#{id} record is not found.", nameof(id));
            }
        }

        /// <inheritdoc/>
        public List<int> Update(List<KeyValuePair<string, string>> newRecordParameters, List<KeyValuePair<string, string>> searchOptions)
        {
            const string invalidValueForSearchOptionMessage = "Invalid search parameter value.";
            const string invalidValueForNewRecordParameterMessage = "Invalid value for new record parameter.";
            const string noRecordsFoundMessage = "No records were found with the specified keys.";

            if (newRecordParameters == null)
            {
                throw new ArgumentNullException(nameof(newRecordParameters));
            }

            if (searchOptions == null)
            {
                throw new ArgumentNullException(nameof(searchOptions));
            }

            this.Purge(false);

            var validator = this.InputValidator;
            var identifiersOfRecordsToUpdate = new List<int>();
            var recordsToUpdate = this.GetRecords().ToList<FileCabinetRecord>().FindAll(delegate(FileCabinetRecord fileCabinetRecord)
            {
                bool isRecordToUpdate = false;

                searchOptions.ForEach(delegate(KeyValuePair<string, string> searchOptionPair)
                {
                    switch (searchOptionPair.Key)
                    {
                        case "ID":
                            if (int.TryParse(searchOptionPair.Value, out int id))
                            {
                                isRecordToUpdate = fileCabinetRecord.Id == id ? true : false;
                            }
                            else
                            {
                                throw new ArgumentException(invalidValueForSearchOptionMessage);
                            }

                            break;
                        case "FIRSTNAME":
                            isRecordToUpdate = fileCabinetRecord.FirstName == searchOptionPair.Value ? true : false;

                            break;
                        case "LASTNAME":
                            isRecordToUpdate = fileCabinetRecord.LastName == searchOptionPair.Value ? true : false;

                            break;
                        case "DATEOFBIRTH":
                            if (DateTime.TryParse(searchOptionPair.Value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth))
                            {
                                isRecordToUpdate = fileCabinetRecord.DateOfBirth == dateOfBirth ? true : false;
                            }
                            else
                            {
                                throw new ArgumentException(invalidValueForSearchOptionMessage);
                            }

                            break;
                        case "WALLET":
                            if (decimal.TryParse(searchOptionPair.Value, out decimal wallet))
                            {
                                isRecordToUpdate = fileCabinetRecord.Wallet == wallet ? true : false;
                            }
                            else
                            {
                                throw new ArgumentException(invalidValueForSearchOptionMessage);
                            }

                            break;
                        case "MARITALSTATUS":
                            if (char.TryParse(searchOptionPair.Value, out char maritalStatus))
                            {
                                isRecordToUpdate = fileCabinetRecord.MaritalStatus == maritalStatus ? true : false;
                            }
                            else
                            {
                                throw new ArgumentException(invalidValueForSearchOptionMessage);
                            }

                            break;
                        case "HEIGHT":
                            if (short.TryParse(searchOptionPair.Value, out short height))
                            {
                                isRecordToUpdate = fileCabinetRecord.Height == height ? true : false;
                            }
                            else
                            {
                                throw new ArgumentException(invalidValueForSearchOptionMessage);
                            }

                            break;
                        default:
                            throw new ArgumentException("Invalid key to update the record.");
                    }
                });

                return isRecordToUpdate;
            });

            if (recordsToUpdate.Count > 0)
            {
                recordsToUpdate.ForEach(delegate(FileCabinetRecord fileCabinetRecord)
                {
                    var offset = this.identifierDictionary[fileCabinetRecord.Id];
                    var oldRecord = new FileCabinetRecord(fileCabinetRecord);

                    identifiersOfRecordsToUpdate.Add(fileCabinetRecord.Id);

                    newRecordParameters.ForEach(delegate(KeyValuePair<string, string> newRecordParameter)
                    {
                        switch (newRecordParameter.Key)
                        {
                            case "ID":
                                throw new ArgumentException("Id update is not supported.");

                            case "FIRSTNAME":
                                if (validator.FirstNameValidator(newRecordParameter.Value).Item1)
                                {
                                    fileCabinetRecord.FirstName = newRecordParameter.Value;
                                }
                                else
                                {
                                    throw new ArgumentException($"{invalidValueForNewRecordParameterMessage}: {newRecordParameter.Key}");
                                }

                                break;
                            case "LASTNAME":
                                if (validator.LastNameValidator(newRecordParameter.Value).Item1)
                                {
                                    fileCabinetRecord.LastName = newRecordParameter.Value;
                                }
                                else
                                {
                                    throw new ArgumentException($"{invalidValueForNewRecordParameterMessage}: {newRecordParameter.Key}");
                                }

                                break;
                            case "DATEOFBIRTH":
                                if (DateTime.TryParse(newRecordParameter.Value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth) &&
                                    validator.DateOfBirthValidator(dateOfBirth).Item1)
                                {
                                    fileCabinetRecord.DateOfBirth = dateOfBirth;
                                }
                                else
                                {
                                    throw new ArgumentException($"{invalidValueForNewRecordParameterMessage}: {newRecordParameter.Key}");
                                }

                                break;
                            case "WALLET":
                                if (decimal.TryParse(newRecordParameter.Value, out decimal wallet) &&
                                    validator.WalletValidator(wallet).Item1)
                                {
                                    fileCabinetRecord.Wallet = wallet;
                                }
                                else
                                {
                                    throw new ArgumentException($"{invalidValueForNewRecordParameterMessage}: {newRecordParameter.Key}");
                                }

                                break;
                            case "MARITALSTATUS":
                                if (char.TryParse(newRecordParameter.Value, out char maritalStatus) &&
                                    validator.MaritalStatusValidator(maritalStatus).Item1)
                                {
                                    fileCabinetRecord.MaritalStatus = maritalStatus;
                                }
                                else
                                {
                                    throw new ArgumentException($"{invalidValueForNewRecordParameterMessage}: {newRecordParameter.Key}");
                                }

                                break;
                            case "HEIGHT":
                                if (short.TryParse(newRecordParameter.Value, out short height) &&
                                    validator.HeightValidator(height).Item1)
                                {
                                    fileCabinetRecord.Height = height;
                                }
                                else
                                {
                                    throw new ArgumentException($"{invalidValueForNewRecordParameterMessage}: {newRecordParameter.Key}");
                                }

                                break;
                            default:
                                throw new ArgumentException("Invalid key to update the record.");
                        }
                    });

                    this.RemoveEntryFromDictionaries(oldRecord, offset);
                    this.AddEntryToDictionaries(fileCabinetRecord, offset);

                    var recordBytes = FileCabinetFileSystemService.FileCabinetRecordToBytes(fileCabinetRecord);
                    this.fileStream.Seek(offset, SeekOrigin.Begin);
                    this.fileStream.Write(recordBytes, 0, recordBytes.Length);
                    this.fileStream.Flush();
                });

                return identifiersOfRecordsToUpdate;
            }
            else
            {
                throw new ArgumentException(noRecordsFoundMessage);
            }
        }

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        /// <param name="isDisplayResult">Is the result dispalyed.</param>
        public void Purge(bool isDisplayResult = true)
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

            if (isDisplayResult)
            {
                Console.WriteLine($"Data file processing is completed: {totalNumberOfRecords - records.Count} of {totalNumberOfRecords} records were purged.");
                Console.WriteLine();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectByCriteria(SearchProperties searchProperties)
        {
            if (searchProperties is null)
            {
                throw new ArgumentNullException(nameof(searchProperties));
            }

            var selectedRecords = this.GetRecordsByCriteria(searchProperties);
            if (selectedRecords.Any())
            {
                return new FilesystemIterator(this.fileStream, selectedRecords);
            }
            else
            {
                return new FilesystemIterator(this.fileStream, new List<long>());
            }
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
                    importedRecordsCount++;
                    try
                    {
                        this.EditRecord(importedRecord.Id, importedRecord);
                    }
                    catch (ArgumentException)
                    {
                        this.CreateRecord(importedRecord, true);
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
        public List<int> Delete(string key, string value)
        {
            const string noRecordsFoundMessage = "No records were found with the specified key.";
            const string invalidValueMessage = "Invalid value for the specified key.";

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            List<long> offsetsOfRecordsToDelete = null;
            List<int> identifiersOfRecordsToDelete = new List<int>();

            switch (key)
            {
                case "ID":
                    if (int.TryParse(value, out int id))
                    {
                        var offsetOfRecordToDelete = this.identifierDictionary[id];
                        offsetsOfRecordsToDelete = new List<long>();
                        offsetsOfRecordsToDelete.Add(offsetOfRecordToDelete);
                    }
                    else
                    {
                        throw new ArgumentException(noRecordsFoundMessage);
                    }

                    break;
                case "FIRSTNAME":
                    if (this.firstNameDictionary.ContainsKey(value.ToUpperInvariant()))
                    {
                        offsetsOfRecordsToDelete = new List<long>(this.firstNameDictionary[value.ToUpperInvariant()]);
                    }
                    else
                    {
                        throw new ArgumentException(noRecordsFoundMessage);
                    }

                    break;
                case "LASTNAME":
                    if (this.lastNameDictionary.ContainsKey(value.ToUpperInvariant()))
                    {
                        offsetsOfRecordsToDelete = new List<long>(this.lastNameDictionary[value.ToUpperInvariant()]);
                    }
                    else
                    {
                        throw new ArgumentException(noRecordsFoundMessage);
                    }

                    break;
                case "DATEOFBIRTH":
                    if (DateTime.TryParse(value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth))
                    {
                        if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
                        {
                            offsetsOfRecordsToDelete = new List<long>(this.dateOfBirthDictionary[dateOfBirth]);
                        }
                        else
                        {
                            throw new ArgumentException(noRecordsFoundMessage);
                        }
                    }
                    else
                    {
                        throw new ArgumentException(invalidValueMessage);
                    }

                    break;
                case "WALLET":
                    if (decimal.TryParse(value, out decimal wallet))
                    {
                        if (this.walletDictionary.ContainsKey(wallet))
                        {
                            offsetsOfRecordsToDelete = new List<long>(this.walletDictionary[wallet]);
                        }
                        else
                        {
                            throw new ArgumentException(noRecordsFoundMessage);
                        }
                    }
                    else
                    {
                        throw new ArgumentException(invalidValueMessage);
                    }

                    break;
                case "MARITALSTATUS":
                    if (char.TryParse(value, out char maritalStatus))
                    {
                        if (this.maritalStatusDictionary.ContainsKey(maritalStatus))
                        {
                            offsetsOfRecordsToDelete = new List<long>(this.maritalStatusDictionary[maritalStatus]);
                        }
                        else
                        {
                            throw new ArgumentException(noRecordsFoundMessage);
                        }
                    }
                    else
                    {
                        throw new ArgumentException(invalidValueMessage);
                    }

                    break;
                case "HEIGHT":
                    if (short.TryParse(value, out short height))
                    {
                        if (this.heightDictionary.ContainsKey(height))
                        {
                            offsetsOfRecordsToDelete = new List<long>(this.heightDictionary[height]);
                        }
                        else
                        {
                            throw new ArgumentException(noRecordsFoundMessage);
                        }
                    }
                    else
                    {
                        throw new ArgumentException(invalidValueMessage);
                    }

                    break;
                default:
                    throw new ArgumentException("Invalid key to delete the record.");
            }

            offsetsOfRecordsToDelete.ForEach(delegate(long offset)
            {
                var deletedRecord = new FileCabinetRecord();
                if (this.TryGetRecordByOffset(offset, ref deletedRecord))
                {
                    identifiersOfRecordsToDelete.Add(deletedRecord.Id);
                    this.RemoveEntryFromDictionaries(deletedRecord, offset);

                    this.fileStream.Seek(offset, SeekOrigin.Begin);
                    this.fileStream.WriteByte(1);
                }
            });

            this.UpdateLastRecordId();
            this.UpdateDictionaries();

            return identifiersOfRecordsToDelete;
        }

        /// <summary>
        /// Gets a record from a file and returns a value indicating whether the retrieval was successful.
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// <param name="fileCabinetRecord">FileCabinetRecord.</param>
        /// <returns>Returns true if the record was retrieved.</returns>
        public bool TryGetRecordByOffset(long offset, ref FileCabinetRecord fileCabinetRecord)
        {
            if (offset % RecordSize != 0)
            {
                throw new ArgumentException($"The offset must be a multiple of {RecordSize}", nameof(offset));
            }

            var recordBuffer = new byte[RecordSize];
            FileCabinetRecord receivedRecord;

            this.fileStream.Seek(offset, SeekOrigin.Begin);
            this.fileStream.Read(recordBuffer, 0, RecordSize);

            if (BytesToFileCabinetRecord(recordBuffer, out receivedRecord))
            {
                fileCabinetRecord = receivedRecord;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Releases fileStream, binaryReader, binaryWriter.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
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

        private static void GetFinalListOfOverlap(List<string> signs, List<List<long>> selectedRecords)
        {
            while (selectedRecords.Count > 1)
            {
                if (signs.Contains(SignAND))
                {
                    int positionAND = signs.IndexOf(SignAND);
                    signs.RemoveAt(positionAND);
                    List<long> andRezult = new List<long>();
                    foreach (var record in selectedRecords[positionAND + 1])
                    {
                        if (selectedRecords[positionAND].Contains(record))
                        {
                            andRezult.Add(record);
                        }
                    }

                    selectedRecords[positionAND + 1] = andRezult;
                    selectedRecords.RemoveAt(positionAND);
                }
                else
                {
                    int positionOR = signs.IndexOf(SignOR);
                    signs.RemoveAt(positionOR);
                    List<long> orRezult = selectedRecords[positionOR + 1];
                    foreach (var record in selectedRecords[positionOR])
                    {
                        if (!selectedRecords[positionOR + 1].Contains(record))
                        {
                            orRezult.Add(record);
                        }
                    }

                    selectedRecords[positionOR + 1] = orRezult;
                    selectedRecords.RemoveAt(positionOR);
                }
            }
        }

        private List<long> GetRecordsByCriteria(SearchProperties searchProperties)
        {
            var selectedRecords = new List<List<long>>();
            foreach (var property in searchProperties.List)
            {
                var fieldName = property.Item1;
                var offsets = this.GetOffsetsByFieldName(property, fieldName);
                selectedRecords.Add(offsets);
            }

            GetFinalListOfOverlap(new List<string>(searchProperties.Signs), selectedRecords);
            return selectedRecords[0];
        }

        private List<long> GetOffsetsByFieldName(Tuple<string, object> property, string fieldName)
        {
            var records = new List<long>();
            switch (true)
            {
                case bool isIdField when fieldName.Equals(nameof(FileCabinetRecord.Id), StringComparison.CurrentCultureIgnoreCase):
                    if (this.identifierDictionary.ContainsKey((int)property.Item2))
                    {
                        records.Add(this.identifierDictionary[(int)property.Item2]);
                    }

                    break;

                case bool isFirstNameField when fieldName.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.CurrentCultureIgnoreCase):
                    if (this.firstNameDictionary.ContainsKey(((string)property.Item2).ToUpperInvariant()))
                    {
                        records = this.firstNameDictionary[((string)property.Item2).ToUpperInvariant()];
                    }

                    break;

                case bool isLastNameField when fieldName.Equals(nameof(FileCabinetRecord.LastName), StringComparison.CurrentCultureIgnoreCase):
                    if (this.lastNameDictionary.ContainsKey(((string)property.Item2).ToUpperInvariant()))
                    {
                        records = this.lastNameDictionary[((string)property.Item2).ToUpperInvariant()];
                    }

                    break;

                case bool isDateField when fieldName.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.CurrentCultureIgnoreCase):
                    if (this.dateOfBirthDictionary.ContainsKey((DateTime)property.Item2))
                    {
                        records = this.dateOfBirthDictionary[(DateTime)property.Item2];
                    }

                    break;

                case bool isBudgetField when fieldName.Equals(nameof(FileCabinetRecord.Wallet), StringComparison.CurrentCultureIgnoreCase):
                    if (this.walletDictionary.ContainsKey((decimal)property.Item2))
                    {
                        records = this.walletDictionary[(decimal)property.Item2];
                    }

                    break;

                case bool isMaritalStatusField when fieldName.Equals(nameof(FileCabinetRecord.MaritalStatus), StringComparison.CurrentCultureIgnoreCase):
                    if (this.maritalStatusDictionary.ContainsKey((char)property.Item2))
                    {
                        records = this.maritalStatusDictionary[(char)property.Item2];
                    }

                    break;

                case bool isAgeField when fieldName.Equals(nameof(FileCabinetRecord.Height), StringComparison.CurrentCultureIgnoreCase):
                    if (this.heightDictionary.ContainsKey((short)property.Item2))
                    {
                        records = this.heightDictionary[(short)property.Item2];
                    }

                    break;
            }

            return records;
        }

        private bool TryGetRecordWithId(int id, ref FileCabinetRecord fileCabinetRecord)
        {
            var isSuccess = false;

            if (this.identifierDictionary.ContainsKey(id))
            {
                FileCabinetRecord receivedRecord = null;
                isSuccess = this.TryGetRecordByOffset(this.identifierDictionary[id], ref receivedRecord);
                if (receivedRecord.Id == id)
                {
                    return isSuccess;
                }
            }

            return isSuccess;
        }

        private int GetLastRecordId()
        {
            var maxRecordId = 0;
            if (this.identifierDictionary.Count > 0)
            {
                return this.identifierDictionary.Keys.Where(key => key > maxRecordId)
                                                     .Max(key => key);
            }
            else
            {
                return maxRecordId;
            }
        }

        private void RemoveEntryFromDictionaries(FileCabinetRecord fileCabinetRecord, long offset)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            if (this.identifierDictionary.ContainsKey(fileCabinetRecord.Id) &&
                this.firstNameDictionary.ContainsKey(fileCabinetRecord.FirstName.ToUpperInvariant()) &&
                this.lastNameDictionary.ContainsKey(fileCabinetRecord.LastName.ToUpperInvariant()) &&
                this.dateOfBirthDictionary.ContainsKey(fileCabinetRecord.DateOfBirth) &&
                this.walletDictionary.ContainsKey(fileCabinetRecord.Wallet) &&
                this.maritalStatusDictionary.ContainsKey(fileCabinetRecord.MaritalStatus) &&
                this.heightDictionary.ContainsKey(fileCabinetRecord.Height))
            {
                this.identifierDictionary.Remove(fileCabinetRecord.Id);
                this.firstNameDictionary[fileCabinetRecord.FirstName.ToUpperInvariant()].Remove(offset);
                this.lastNameDictionary[fileCabinetRecord.LastName.ToUpperInvariant()].Remove(offset);
                this.dateOfBirthDictionary[fileCabinetRecord.DateOfBirth].Remove(offset);
                this.walletDictionary[fileCabinetRecord.Wallet].Remove(offset);
                this.maritalStatusDictionary[fileCabinetRecord.MaritalStatus].Remove(offset);
                this.heightDictionary[fileCabinetRecord.Height].Remove(offset);
            }
        }

        private void AddEntryToDictionaries(FileCabinetRecord fileCabinetRecord, long offset)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            if (!this.identifierDictionary.ContainsKey(fileCabinetRecord.Id))
            {
                this.identifierDictionary.Add(fileCabinetRecord.Id, offset);
            }

            if (!this.firstNameDictionary.ContainsKey(fileCabinetRecord.FirstName.ToUpperInvariant()))
            {
                this.firstNameDictionary.Add(fileCabinetRecord.FirstName.ToUpperInvariant(), new List<long>());
            }

            if (!this.lastNameDictionary.ContainsKey(fileCabinetRecord.LastName.ToUpperInvariant()))
            {
                this.lastNameDictionary.Add(fileCabinetRecord.LastName.ToUpperInvariant(), new List<long>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(fileCabinetRecord.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(fileCabinetRecord.DateOfBirth, new List<long>());
            }

            if (!this.walletDictionary.ContainsKey(fileCabinetRecord.Wallet))
            {
                this.walletDictionary.Add(fileCabinetRecord.Wallet, new List<long>());
            }

            if (!this.maritalStatusDictionary.ContainsKey(fileCabinetRecord.MaritalStatus))
            {
                this.maritalStatusDictionary.Add(fileCabinetRecord.MaritalStatus, new List<long>());
            }

            if (!this.heightDictionary.ContainsKey(fileCabinetRecord.Height))
            {
                this.heightDictionary.Add(fileCabinetRecord.Height, new List<long>());
            }

            this.identifierDictionary[fileCabinetRecord.Id] = offset;
            this.firstNameDictionary[fileCabinetRecord.FirstName.ToUpperInvariant()].Add(offset);
            this.lastNameDictionary[fileCabinetRecord.LastName.ToUpperInvariant()].Add(offset);
            this.dateOfBirthDictionary[fileCabinetRecord.DateOfBirth].Add(offset);
            this.walletDictionary[fileCabinetRecord.Wallet].Add(offset);
            this.maritalStatusDictionary[fileCabinetRecord.MaritalStatus].Add(offset);
            this.heightDictionary[fileCabinetRecord.Height].Add(offset);
        }

        private ReadOnlyCollection<FileCabinetRecord> GetRecordsFromAListOfOffsets(List<long> offsets)
        {
            if (offsets == null)
            {
                throw new ArgumentNullException(nameof(offsets));
            }

            var foundRecords = new List<FileCabinetRecord>();

            foreach (var offset in offsets)
            {
                var recordBuffer = new byte[RecordSize];
                this.fileStream.Seek(offset, SeekOrigin.Begin);
                this.fileStream.Read(recordBuffer, 0, recordBuffer.Length);
                var temporaryRecord = new FileCabinetRecord();
                if (BytesToFileCabinetRecord(recordBuffer, out temporaryRecord))
                {
                    foundRecords.Add(temporaryRecord);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(foundRecords);
        }

        private void UpdateLastRecordId()
        {
            if (this.identifierDictionary.Count > 0)
            {
                this.lastRecordId = this.identifierDictionary.Keys.Max();
            }
        }

        private void UpdateDictionaries()
        {
            this.identifierDictionary = new Dictionary<int, long>();
            this.firstNameDictionary = new Dictionary<string, List<long>>();
            this.lastNameDictionary = new Dictionary<string, List<long>>();
            this.dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();
            this.walletDictionary = new Dictionary<decimal, List<long>>();
            this.maritalStatusDictionary = new Dictionary<char, List<long>>();
            this.heightDictionary = new Dictionary<short, List<long>>();

            var localRecords = this.GetRecords();
            for (int i = 0; i < localRecords.Count; i++)
            {
                this.AddEntryToDictionaries(localRecords[i], i * RecordSize);
            }
        }
    }
}
