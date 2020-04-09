using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents an abstract service that stores records with personal information about a person.
    /// </summary>
    public class FileCabinetService
    {
        /// <summary>
        /// Culture.
        /// </summary>
        protected static readonly CultureInfo CultureEnUS = new CultureInfo("en-US");

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Displays a list of entries.
        /// </summary>
        /// <param name="records"> List of entries. </param>
        public static void DisplayRecords(FileCabinetRecord[] records)
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
        /// Creates a record with personal information about a person and adds it to the list.
        /// </summary>
        /// <param name="recordParameters"> FileCabinetRecord fields. </param>
        /// <returns> Identifier of the new record. </returns>
        public int CreateRecord(RecordParameters recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            this.ValidateParameters(recordParameters);

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

        /// <summary>
        /// Gets a list of entries.
        /// </summary>
        /// <returns> List of entries. </returns>
        public FileCabinetRecord[] GetRecords()
        {
            return new List<FileCabinetRecord>(this.list).ToArray();
        }

        /// <summary>
        /// Get statistics about records.
        /// </summary>
        /// <returns> Number of records. </returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Edits a record by ID.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <param name="recordParameters"> FileCabinetRecord fields. </param>
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

            this.ValidateParameters(recordParameters);

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

        /// <summary>
        /// Gets a list of entries by first name.
        /// </summary>
        /// <param name="firstName"> The first name. </param>
        /// <returns> List of entries. </returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
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
                return this.firstNameDictionary[firstName.ToUpperInvariant()].ToArray();
            }
            else
            {
                return Array.Empty<FileCabinetRecord>();
            }
        }

        /// <summary>
        /// Gets a list of entries by last name.
        /// </summary>
        /// <param name="lastName"> The last name. </param>
        /// <returns> List of entries. </returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
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
                return this.lastNameDictionary[lastName.ToUpperInvariant()].ToArray();
            }
            else
            {
                return Array.Empty<FileCabinetRecord>();
            }
        }

        /// <summary>
        /// Gets a list of entries by date of birth.
        /// </summary>
        /// <param name="dateOfBirth"> The date of birth. </param>
        /// <returns> List of entries. </returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
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
                return this.dateOfBirthDictionary[date].ToArray();
            }
            else
            {
                return Array.Empty<FileCabinetRecord>();
            }
        }

        /// <summary>
        /// Enter personal information about the person to record.
        /// </summary>
        /// <returns> RecordParameters. </returns>
        public virtual RecordParameters SetInformationToRecord()
        {
            const int informationAboutMaritalStatus = 0;

            Console.Write("First Name: ");
            var firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            var lastName = Console.ReadLine();

            Console.Write("Date of birth (MM/DD/YYYY): ");
            var dateOfBirth = DateTime.Parse(Console.ReadLine(), CultureEnUS);

            Console.WriteLine("Wallet (from 0): ");
            var wallet = decimal.Parse(Console.ReadLine(), CultureEnUS);

            Console.WriteLine("Marital status ('M' - married, 'U' - unmarried): ");
            var maritalStatus = char.MinValue;
            var married = Console.ReadLine();
            if (married.Length > 0)
            {
                maritalStatus = married[informationAboutMaritalStatus];
            }

            Console.WriteLine("Height (more than 0): ");
            var height = short.Parse(Console.ReadLine(), CultureEnUS);

            return new RecordParameters(firstName, lastName, dateOfBirth, wallet, maritalStatus, height);
        }

        /// <summary>
        /// Throws an exception if the record parameters are incorrect.
        /// </summary>
        /// <param name="recordParameters"> Record fields. </param>
        protected virtual void ValidateParameters(RecordParameters recordParameters)
        {
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