﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Represents an abstract service that stores records with personal information about a person.
    /// </summary>
    public class FileCabinetService : IFileCabinetService
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validationType"> The validation type. </param>
        public FileCabinetService(string validationType)
        {
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

        /// <summary>
        /// Displays a list of entries.
        /// </summary>
        /// <param name="records"> Collection of entries. </param>
        public static void DisplayRecords(ReadOnlyCollection<FileCabinetRecord> records)
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

        /// <summary>
        /// Enter personal information about the person to record.
        /// </summary>
        /// <returns> RecordParameters. </returns>
        public RecordParameters SetInformationToRecord()
        {
            Console.Write("First Name: ");
            var firstName = ReadInput(InputConverters.StringConverter, this.validator.FirstNameValidator);

            Console.Write("Last Name: ");
            var lastName = ReadInput(InputConverters.StringConverter, this.validator.LastNameValidator);

            Console.Write("Date of birth (MM/DD/YYYY): ");
            var dateOfBirth = ReadInput(InputConverters.DateConverter, this.validator.DateOfBirthValidator);

            Console.WriteLine("Wallet: ");
            var wallet = ReadInput(InputConverters.WalletConverter, this.validator.WalletValidator);

            Console.WriteLine("Marital status ('M' - married, 'U' - unmarried): ");
            var maritalStatus = ReadInput(InputConverters.MaritalStatusConverter, this.validator.MaritalStatusValidator);

            Console.WriteLine("Height: ");
            var height = ReadInput(InputConverters.HeightConverter, this.validator.HeightValidator);

            return new RecordParameters(firstName, lastName, dateOfBirth, wallet, maritalStatus, height);
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameters recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            this.validator.ValidateParameters(recordParameters);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
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
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.list.Count;
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

            this.validator.ValidateParameters(recordParameters);

            foreach (var record in this.list)
            {
                if (record.Id == id)
                {
                    this.firstNameDictionary[record.FirstName.ToUpperInvariant()].Remove(record);
                    this.lastNameDictionary[record.LastName.ToUpperInvariant()].Remove(record);
                    this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);

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

            if (this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.firstNameDictionary[firstName.ToUpperInvariant()]);
            }
            else
            {
                return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
            }
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

            if (this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.lastNameDictionary[lastName.ToUpperInvariant()]);
            }
            else
            {
                return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
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

            if (this.dateOfBirthDictionary.ContainsKey(date))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.dateOfBirthDictionary[date]);
            }
            else
            {
                return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
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
    }
}