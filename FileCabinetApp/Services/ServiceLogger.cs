using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using FileCabinetApp.Iterators;
using FileCabinetApp.Records;
using FileCabinetApp.Validators.InputValidator;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// File cabinet service that logs information about service method calls and passed parameters.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private const string FileForLog = "log.txt";

        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public ServiceLogger(IFileCabinetService fileCabinetService)
        {
            if (fileCabinetService == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetService));
            }

            this.service = fileCabinetService;
        }

        /// <inheritdoc/>
        public IInputValidator InputValidator => this.service.InputValidator;

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            var newRecordId = this.service.CreateRecord(recordParameters);

            Log($"Calling {nameof(this.service.CreateRecord)}() with" +
                $"FirstName = '{recordParameters.FirstName}', " +
                $"LastName = '{recordParameters.LastName}', " +
                $"DateOfBirth = '{recordParameters.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"Wallet = '{recordParameters.Wallet}', " +
                $"MaritalStatus = '{recordParameters.MaritalStatus}', " +
                $"Height = '{recordParameters.Height}'");
            Log($"{nameof(this.service.CreateRecord)}() returned '{newRecordId}'");

            return newRecordId;
        }

        /// <inheritdoc/>
        public int Insert(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            var newRecordId = this.service.Insert(fileCabinetRecord);

            Log($"Calling {nameof(this.service.Insert)}() with" +
                $"FirstName = '{fileCabinetRecord.FirstName}', " +
                $"LastName = '{fileCabinetRecord.LastName}', " +
                $"DateOfBirth = '{fileCabinetRecord.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"Wallet = '{fileCabinetRecord.Wallet}', " +
                $"MaritalStatus = '{fileCabinetRecord.MaritalStatus}', " +
                $"Height = '{fileCabinetRecord.Height}'");
            Log($"{nameof(this.service.CreateRecord)}() returned '{newRecordId}'");

            return newRecordId;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordParameters recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            this.service.EditRecord(id, recordParameters);

            Log($"Calling {nameof(this.service.EditRecord)}() with" +
                $"FirstName = '{recordParameters.FirstName}', " +
                $"LastName = '{recordParameters.LastName}', " +
                $"DateOfBirth = '{recordParameters.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"Wallet = '{recordParameters.Wallet}', " +
                $"MaritalStatus = '{recordParameters.MaritalStatus}', " +
                $"Height = '{recordParameters.Height}'");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            if (string.IsNullOrEmpty(dateOfBirth))
            {
                throw new ArgumentNullException(nameof(dateOfBirth));
            }

            var collectionOfFoundRecords = this.service.FindByDateOfBirth(dateOfBirth);

            Log($"Calling {nameof(this.service.FindByDateOfBirth)}() with" +
                $"DateOfBirth = '{dateOfBirth}'");

            return collectionOfFoundRecords;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            var collectionOfFoundRecords = this.service.FindByFirstName(firstName);

            Log($"Calling {nameof(this.service.FindByFirstName)}() with" +
                $"FirstName = '{firstName}'");

            return collectionOfFoundRecords;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            var collectionOfFoundRecords = this.service.FindByLastName(lastName);

            Log($"Calling {nameof(this.service.FindByLastName)}() with" +
                $"LastName = '{lastName}'");

            return collectionOfFoundRecords;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var collectionOfReceivedRecods = this.service.GetRecords();

            Log($"Calling {nameof(this.service.GetRecords)}()");
            Log($"{nameof(this.service.GetRecords)}() returned a list of '{collectionOfReceivedRecods.Count}' records");

            return collectionOfReceivedRecods;
        }

        /// <inheritdoc/>
        public (int active, int removed) GetStat()
        {
            var statistics = this.service.GetStat();

            Log($"Calling {nameof(this.service.GetStat)}()");

            return statistics;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var snapshot = this.service.MakeSnapshot();

            Log($"Calling {nameof(this.service.MakeSnapshot)}()");

            return snapshot;
        }

        /// <inheritdoc/>
        public bool Remove(int recordIdForRemove)
        {
            var resultOfRemove = this.service.Remove(recordIdForRemove);

            Log($"Calling {nameof(this.service.Remove)}()");

            return resultOfRemove;
        }

        /// <inheritdoc/>
        public List<int> Delete(string key, string value)
        {
            var identifiersOfDeletedRecords = this.service.Delete(key, value);

            Log($"Calling {nameof(this.service.Delete)}()");

            return identifiersOfDeletedRecords;
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            if (fileCabinetServiceSnapshot == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetServiceSnapshot));
            }

            var numberOfRestoredRecords = this.service.Restore(fileCabinetServiceSnapshot);

            Log($"Calling {nameof(this.service.Restore)}()");
            Log($"{nameof(this.service.Restore)}() returned '{numberOfRestoredRecords}'");

            return numberOfRestoredRecords;
        }

        private static void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            using (TextWriter textWriter = File.AppendText(FileForLog))
            {
                textWriter.WriteLine($"{DateTime.Now.ToLongTimeString()} - {message}");
            }
        }
    }
}
