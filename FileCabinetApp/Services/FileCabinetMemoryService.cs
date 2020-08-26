using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using FileCabinetApp.Iterators;
using FileCabinetApp.Records;
using FileCabinetApp.Validators;
using FileCabinetApp.Validators.InputValidator;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// It is an abstract service in which records with personal information about a person are stored in memory.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        /// <summary>
        /// Culture.
        /// </summary>
        protected static readonly CultureInfo CultureEnUS = new CultureInfo("en-US");

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        private IRecordValidator validator;
        private int lastRecordId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validationType">The validation type.</param>
        public FileCabinetMemoryService(string validationType)
        {
            if (validationType == null)
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

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            this.validator.ValidateParameters(recordParameters);

            var record = new FileCabinetRecord
            {
                Id = ++this.lastRecordId,
                FirstName = recordParameters.FirstName,
                LastName = recordParameters.LastName,
                DateOfBirth = recordParameters.DateOfBirth,
                Wallet = recordParameters.Wallet,
                MaritalStatus = recordParameters.MaritalStatus,
                Height = recordParameters.Height,
            };

            this.list.Add(record);

            this.AddEntryToDictionaries(record);

            return record.Id;
        }

        /// <summary>
        /// Creates a record with personal information about the person and with the specified identifier and adds it to the list.
        /// </summary>
        /// <param name="recordParameters">FileCabinetRecord fields.</param>
        /// <param name="id">Identifier.</param>
        /// <returns>Identifier of the new record.</returns>
        public int CreateRecord(FileCabinetRecord recordParameters, int id)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (id < 0)
            {
                throw new ArgumentException("The record ID must be greater than zero.", nameof(id));
            }

            this.validator.ValidateParameters(recordParameters);

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = recordParameters.FirstName,
                LastName = recordParameters.LastName,
                DateOfBirth = recordParameters.DateOfBirth,
                Wallet = recordParameters.Wallet,
                MaritalStatus = recordParameters.MaritalStatus,
                Height = recordParameters.Height,
            };

            this.list.Add(record);

            this.AddEntryToDictionaries(record);

            return record.Id;
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
                var recordsWithThisId = from record in this.list
                                        where record.Id == fileCabinetRecord.Id
                                        select record;

                if (recordsWithThisId.ToList().Count < 1)
                {
                    var insertedRecordId = this.CreateRecord(fileCabinetRecord, fileCabinetRecord.Id);
                    this.UpdateLastRecordId();
                    return insertedRecordId;
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
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <inheritdoc/>
        public (int active, int removed) GetStat() => (this.list.Count, 0);

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

            this.validator.ValidateParameters(recordParameters);

            foreach (var record in this.list)
            {
                if (record.Id == id)
                {
                    this.RemoveEntryFromDictionaries(record);

                    record.FirstName = recordParameters.FirstName;
                    record.LastName = recordParameters.LastName;
                    record.DateOfBirth = recordParameters.DateOfBirth;
                    record.Wallet = recordParameters.Wallet;
                    record.MaritalStatus = recordParameters.MaritalStatus;
                    record.Height = recordParameters.Height;

                    this.AddEntryToDictionaries(record);

                    return;
                }
            }

            throw new ArgumentException($"#{id} record is not found.", nameof(id));
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

            var identifiersOfRecordsToUpdate = new List<int>();
            var recordsToUpdate = this.list.FindAll(delegate(FileCabinetRecord fileCabinetRecord)
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
                    identifiersOfRecordsToUpdate.Add(fileCabinetRecord.Id);

                    newRecordParameters.ForEach(delegate(KeyValuePair<string, string> newRecordParameter)
                    {
                        switch (newRecordParameter.Key)
                        {
                            case "ID":
                                throw new ArgumentException("Id update is not supported.");

                            case "FIRSTNAME":
                                this.RemoveEntryFromDictionaries(fileCabinetRecord);
                                fileCabinetRecord.FirstName = newRecordParameter.Value;
                                this.AddEntryToDictionaries(fileCabinetRecord);

                                break;
                            case "LASTNAME":
                                this.RemoveEntryFromDictionaries(fileCabinetRecord);
                                fileCabinetRecord.LastName = newRecordParameter.Value;
                                this.AddEntryToDictionaries(fileCabinetRecord);

                                break;
                            case "DATEOFBIRTH":
                                if (DateTime.TryParse(newRecordParameter.Value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth))
                                {
                                    this.RemoveEntryFromDictionaries(fileCabinetRecord);
                                    fileCabinetRecord.DateOfBirth = dateOfBirth;
                                    this.AddEntryToDictionaries(fileCabinetRecord);
                                }
                                else
                                {
                                    throw new ArgumentException(invalidValueForNewRecordParameterMessage);
                                }

                                break;
                            case "WALLET":
                                if (decimal.TryParse(newRecordParameter.Value, out decimal wallet))
                                {
                                    fileCabinetRecord.Wallet = wallet;
                                }
                                else
                                {
                                    throw new ArgumentException(invalidValueForNewRecordParameterMessage);
                                }

                                break;
                            case "MARITALSTATUS":
                                if (char.TryParse(newRecordParameter.Value, out char maritalStatus))
                                {
                                    fileCabinetRecord.MaritalStatus = maritalStatus;
                                }
                                else
                                {
                                    throw new ArgumentException(invalidValueForNewRecordParameterMessage);
                                }

                                break;
                            case "HEIGHT":
                                if (short.TryParse(newRecordParameter.Value, out short height))
                                {
                                    fileCabinetRecord.Height = height;
                                }
                                else
                                {
                                    throw new ArgumentException(invalidValueForNewRecordParameterMessage);
                                }

                                break;
                            default:
                                throw new ArgumentException("Invalid key to delete the record.");
                        }
                    });
                });

                return identifiersOfRecordsToUpdate;
            }
            else
            {
                throw new ArgumentException(noRecordsFoundMessage);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (firstName.Length == 0)
            {
                throw new ArgumentException($"There are no entries with an empty first name.", nameof(firstName));
            }

            if (this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                return new MemoryIterator(this.firstNameDictionary[firstName.ToUpperInvariant()]);
            }
            else
            {
                return new MemoryIterator(new List<FileCabinetRecord>());
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (lastName.Length == 0)
            {
                throw new ArgumentException($"There are no entries with an empty last name.", nameof(lastName));
            }

            if (this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
            {
                return new MemoryIterator(this.lastNameDictionary[lastName.ToUpperInvariant()]);
            }
            else
            {
                return new MemoryIterator(new List<FileCabinetRecord>());
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
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

            if (this.dateOfBirthDictionary.ContainsKey(date))
            {
                return new MemoryIterator(this.dateOfBirthDictionary[date]);
            }
            else
            {
                return new MemoryIterator(new List<FileCabinetRecord>());
            }
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
                this.RemoveEntryFromDictionaries(this.list[indexOfRecordForRemove]);
                this.list.RemoveAt(indexOfRecordForRemove);

                return true;
            }
            else
            {
                return false;
            }
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

            List<FileCabinetRecord> recordsToDelete = new List<FileCabinetRecord>();
            List<int> identifiersOfRecordsToDelete = new List<int>();

            switch (key)
            {
                case "ID":
                    if (int.TryParse(value, out int id))
                    {
                        recordsToDelete = this.list.FindAll(record => record.Id == id);
                    }
                    else
                    {
                        throw new ArgumentException(invalidValueMessage);
                    }

                    break;
                case "FIRSTNAME":
                    recordsToDelete = this.list.FindAll(record => record.FirstName == value);

                    break;
                case "LASTNAME":
                    recordsToDelete = this.list.FindAll(record => record.LastName == value);

                    break;
                case "DATEOFBIRTH":
                    if (DateTime.TryParse(value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth))
                    {
                        recordsToDelete = this.list.FindAll(record => record.DateOfBirth == dateOfBirth);
                    }
                    else
                    {
                        throw new ArgumentException(invalidValueMessage);
                    }

                    break;
                case "WALLET":
                    if (decimal.TryParse(value, out decimal wallet))
                    {
                        recordsToDelete = this.list.FindAll(record => record.Wallet == wallet);
                    }
                    else
                    {
                        throw new ArgumentException(invalidValueMessage);
                    }

                    break;
                case "MARITALSTATUS":
                    if (char.TryParse(value, out char maritalStatus))
                    {
                        recordsToDelete = this.list.FindAll(record => record.MaritalStatus == maritalStatus);
                    }
                    else
                    {
                        throw new ArgumentException(invalidValueMessage);
                    }

                    break;
                case "HEIGHT":
                    if (short.TryParse(value, out short height))
                    {
                        recordsToDelete = this.list.FindAll(record => record.Height == height);
                    }
                    else
                    {
                        throw new ArgumentException(invalidValueMessage);
                    }

                    break;
                default:
                    throw new ArgumentException("Invalid key to delete the record.");
            }

            if (recordsToDelete.Count > 0)
            {
                recordsToDelete.ForEach(delegate(FileCabinetRecord fileCabinetRecord)
                {
                    identifiersOfRecordsToDelete.Add(fileCabinetRecord.Id);
                    this.RemoveEntryFromDictionaries(fileCabinetRecord);
                    this.list.Remove(fileCabinetRecord);
                });

                this.UpdateLastRecordId();

                return identifiersOfRecordsToDelete;
            }
            else
            {
                throw new ArgumentException(noRecordsFoundMessage);
            }
        }

        private bool TryGetIndexOfRecordWithId(int id, out int index)
        {
            index = -1;

            for (int i = 0; i < this.list.Count; i++)
            {
                if (this.list[i].Id == id)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }

        private void RemoveEntryFromDictionaries(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            if (this.firstNameDictionary.ContainsKey(fileCabinetRecord.FirstName.ToUpperInvariant()) &&
                this.lastNameDictionary.ContainsKey(fileCabinetRecord.LastName.ToUpperInvariant()) &&
                this.dateOfBirthDictionary.ContainsKey(fileCabinetRecord.DateOfBirth))
            {
                this.firstNameDictionary[fileCabinetRecord.FirstName.ToUpperInvariant()].Remove(fileCabinetRecord);
                this.lastNameDictionary[fileCabinetRecord.LastName.ToUpperInvariant()].Remove(fileCabinetRecord);
                this.dateOfBirthDictionary[fileCabinetRecord.DateOfBirth].Remove(fileCabinetRecord);
            }
        }

        private void AddEntryToDictionaries(FileCabinetRecord record)
        {
            if (!this.firstNameDictionary.ContainsKey(record.FirstName.ToUpperInvariant()))
            {
                this.firstNameDictionary.Add(record.FirstName.ToUpperInvariant(), new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(record.LastName.ToUpperInvariant()))
            {
                this.lastNameDictionary.Add(record.LastName.ToUpperInvariant(), new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(record.DateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[record.FirstName.ToUpperInvariant()].Add(record);
            this.lastNameDictionary[record.LastName.ToUpperInvariant()].Add(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Add(record);
        }

        private void UpdateLastRecordId()
        {
            if (this.list.Count > 0)
            {
                this.lastRecordId = this.list.Max(record => record.Id);
            }
        }
    }
}