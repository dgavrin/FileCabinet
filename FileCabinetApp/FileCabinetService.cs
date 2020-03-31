using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private static readonly DateTime MinimalDateOfBirth = new DateTime(1950, 1, 1);
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, decimal wallet, char maritalStatus, short height)
        {
            _ = firstName ?? throw new ArgumentNullException(nameof(firstName));
            if (firstName.Length < 2 || firstName.Length > 60 || string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("The minimum length of the first name is 2, the maximum is 60.", nameof(firstName));
            }

            _ = lastName ?? throw new ArgumentNullException(nameof(lastName));
            if (lastName.Length < 2 || lastName.Length > 60 || string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("The minimum length of the last name is 2, the maximum is 60.", nameof(lastName));
            }

            if (dateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(dateOfBirth));
            }

            if (dateOfBirth < MinimalDateOfBirth || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth should be later than January 1, 1950, but earlier than now.", nameof(dateOfBirth));
            }

            if (wallet < 0)
            {
                throw new ArgumentException("Money in the wallet cannot be less than zero.", nameof(wallet));
            }

            if (maritalStatus != 'M' || maritalStatus != 'U')
            {
                throw new ArgumentException("Marital status may be M - married, or U - unmarried.", nameof(maritalStatus));
            }

            if (height < 0)
            {
                throw new ArgumentException("Growth cannot be less than zero.", nameof(height));
            }

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

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return new List<FileCabinetRecord>(this.list).ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}