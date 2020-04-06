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
        private static readonly DateTime MinimalDateOfBirth = new DateTime(1950, 1, 1);
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
                if (record.MaritalStatus == 'M')
                {
                    maritalStatus = "married";
                }

                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {dateOfBirth}, {record.Wallet}$, {maritalStatus}, {record.Height}cm");
            }
        }

        /// <summary>
        /// Creates a record with personal information about a person and adds it to the list.
        /// </summary>
        /// <param name="firstName"> The first name. </param>
        /// <param name="lastName"> The last name. </param>
        /// <param name="dateOfBirth"> The date of birth. </param>
        /// <param name="wallet"> The wallet. </param>
        /// <param name="maritalStatus"> The marital status. </param>
        /// <param name="height"> the height. </param>
        /// <returns> Record id. </returns>
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, decimal wallet, char maritalStatus, short height)
        {
            ValidationCheck(firstName, lastName, dateOfBirth, wallet, maritalStatus, height);
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Wallet = wallet,
                MaritalStatus = maritalStatus,
                Height = height,
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
        /// <param name="firstName"> The new first name. </param>
        /// <param name="lastName"> The new last name. </param>
        /// <param name="dateOfBirth"> The new date of birth. </param>
        /// <param name="wallet"> The new wallet. </param>
        /// <param name="maritalStatus"> The new marital status. </param>
        /// <param name="height"> The new height. </param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, decimal wallet, char maritalStatus, short height)
        {
            if (id < 0)
            {
                throw new ArgumentException($"The {nameof(id)} cannot be less than zero.");
            }

            ValidationCheck(firstName, lastName, dateOfBirth, wallet, maritalStatus, height);

            foreach (var record in this.list)
            {
                if (record.Id == id)
                {
                    this.firstNameDictionary[record.FirstName.ToUpperInvariant()].Remove(record);
                    this.lastNameDictionary[record.LastName.ToUpperInvariant()].Remove(record);
                    this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);

                    record.FirstName = firstName;
                    record.LastName = lastName;
                    record.DateOfBirth = dateOfBirth;
                    record.Wallet = wallet;
                    record.MaritalStatus = maritalStatus;
                    record.Height = height;

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

            if (date < MinimalDateOfBirth || date > DateTime.Now)
            {
                return Array.Empty<FileCabinetRecord>();
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
        /// Throws an exception if the parameters are not correct.
        /// </summary>
        /// <param name="firstName"> The first name. </param>
        /// <param name="lastName"> The last name. </param>
        /// <param name="dateOfBirth"> The date of birth. </param>
        /// <param name="wallet"> The wallet. </param>
        /// <param name="maritalStatus"> The marital status. </param>
        /// <param name="height"> The height. </param>
        private static void ValidationCheck(string firstName, string lastName, DateTime dateOfBirth, decimal wallet, char maritalStatus, short height)
        {
            _ = firstName ?? throw new ArgumentNullException(nameof(firstName), $"The {nameof(firstName)} cannot be null.");
            if (firstName.Length < 2 || firstName.Length > 60 || string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("The minimum length of the first name is 2, the maximum is 60.", nameof(firstName));
            }

            _ = lastName ?? throw new ArgumentNullException(nameof(lastName), $"The {nameof(lastName)} cannot be null.");
            if (lastName.Length < 2 || lastName.Length > 60 || string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("The minimum length of the last name is 2, the maximum is 60.", nameof(lastName));
            }

            if (dateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(dateOfBirth), $"The {nameof(dateOfBirth)} cannot be null.");
            }

            if (dateOfBirth < MinimalDateOfBirth || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth should be later than January 1, 1950, but earlier than now.", nameof(dateOfBirth));
            }

            if (wallet < 0)
            {
                throw new ArgumentException("Money in the wallet cannot be less than zero.", nameof(wallet));
            }

            if (maritalStatus != 'M' && maritalStatus != 'm' && maritalStatus != 'U' && maritalStatus != 'u')
            {
                throw new ArgumentException("Marital status may be M - married, or U - unmarried.", nameof(maritalStatus));
            }

            if (height < 0)
            {
                throw new ArgumentException("Growth cannot be less than zero.", nameof(height));
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
    }
}