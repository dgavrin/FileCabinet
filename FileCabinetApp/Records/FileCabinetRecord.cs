﻿using System;
using System.Globalization;

namespace FileCabinetApp.Records
{
    /// <summary>
    /// Presentation of a record in a file cabinet.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        public FileCabinetRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        /// <param name="fileCabinetRecord">FileCabinetRecord.</param>
        public FileCabinetRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            this.Id = fileCabinetRecord.Id;
            this.FirstName = fileCabinetRecord.FirstName;
            this.LastName = fileCabinetRecord.LastName;
            this.DateOfBirth = fileCabinetRecord.DateOfBirth;
            this.Wallet = fileCabinetRecord.Wallet;
            this.MaritalStatus = fileCabinetRecord.MaritalStatus;
            this.Height = fileCabinetRecord.Height;
        }

        /// <summary>
        /// Gets or sets the identifier of the record in the file cabinet.
        /// </summary>
        /// <value>The indentifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the record in the file cabinet.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the record in the file cabinet.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birht of the record in the file cabinet.
        /// </summary>
        /// <value>The date of birth.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the wallet of the record in the file cabinet.
        /// </summary>
        /// <value>The wallet.</value>
        public decimal Wallet { get; set; }

        /// <summary>
        /// Gets or sets the marital status of the record in the file cabinet.
        /// </summary>
        /// <value>The marital status.</value>
        public char MaritalStatus { get; set; }

        /// <summary>
        /// Gets or sets the height of the record in the file cabinet.
        /// </summary>
        /// <value>The height.</value>
        public short Height { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var dateOfBirth = this.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));

            var maritalStatus = "unmarried";
            if (this.MaritalStatus == 'M' || this.MaritalStatus == 'm')
            {
                maritalStatus = "married";
            }

            return $"#{this.Id}, {this.FirstName}, {this.LastName}, {dateOfBirth}, {this.Wallet}$, {maritalStatus}, {this.Height}cm";
        }
    }
}