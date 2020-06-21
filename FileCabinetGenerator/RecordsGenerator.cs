using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Class for generating records.
    /// </summary>
    public class RecordsGenerator
    {
        private const int MaxGeneratedStringLength = 60;
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";

        private static Random random = new Random();

        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private int currentId;
        private int recordsAmount;
        private bool isGenerate = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsGenerator"/> class.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="amount">Amount of records.</param>
        public RecordsGenerator(int id, int amount)
        {
            if (id < 1)
            {
                throw new ArgumentException("Id should be more than 1.", nameof(id));
            }

            if (amount < 1)
            {
                throw new ArgumentException("Records amount should be more than 1.", nameof(amount));
            }

            this.currentId = id;
            this.recordsAmount = amount;
        }

        /// <summary>
        /// Generates a specified number of records.
        /// </summary>
        public void GenerateRecords()
        {
            if (!this.isGenerate)
            {
                for (int i = 0; i < this.recordsAmount; i++)
                {
                    this.list.Add(this.GenerateRecord());
                }

                this.isGenerate = true;
            }
            else
            {
                Console.WriteLine("Records are already generated.");
            }
        }

        private static string GenerateName()
        {
            var generatedString = new StringBuilder();
            var generatedStringLength = RecordsGenerator.random.Next(2, MaxGeneratedStringLength + 1);

            generatedString.Append(UppercaseLetters[RecordsGenerator.random.Next(0, 26)]);
            for (int i = 1; i < generatedStringLength; i++)
            {
                generatedString.Append(LowercaseLetters[RecordsGenerator.random.Next(0, 26)]);
            }

            return generatedString.ToString();
        }

        private static char GenerateMaritalStatus()
        {
            var isMarried = RecordsGenerator.random.Next(0, 2);

            return isMarried == 1 ? 'm' : 'u';
        }

        private static DateTime GenerateDateOfBirth()
        {
            DateTime newDateOfBirth;
            do
            {
                var year = RecordsGenerator.random.Next(1950, DateTime.Now.Year + 1);
                var month = RecordsGenerator.random.Next(1, 13);
                var day = 0;

                if ((year % 4) == 0 && month == 2)
                {
                    day = RecordsGenerator.random.Next(1, 30);
                }
                else if ((year % 4) != 0 && month == 2)
                {
                    day = RecordsGenerator.random.Next(1, 29);
                }
                else if (month == 4 || month == 6 || month == 9 || month == 11)
                {
                    day = RecordsGenerator.random.Next(1, 31);
                }
                else
                {
                    day = RecordsGenerator.random.Next(1, 32);
                }

                newDateOfBirth = new DateTime(year, month, day);
            }
            while (newDateOfBirth > DateTime.Now);

            return newDateOfBirth;
        }

        private FileCabinetRecord GenerateRecord()
        {
            FileCabinetRecord newRecord = new FileCabinetRecord()
            {
                Id = this.currentId++,
                FirstName = GenerateName(),
                LastName = GenerateName(),
                DateOfBirth = GenerateDateOfBirth(),
                Wallet = RecordsGenerator.random.Next(0, int.MaxValue),
                MaritalStatus = GenerateMaritalStatus(),
                Height = (short)RecordsGenerator.random.Next(0, 251),
            };

            return newRecord;
        }
    }
}
