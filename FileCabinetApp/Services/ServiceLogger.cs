using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
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
            this.service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        /// <inheritdoc/>
        public IInputValidator InputValidator => this.service.InputValidator;

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord recordParameters, bool useId = false)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            int newRecordId;
            try
            {
                newRecordId = this.service.CreateRecord(recordParameters, useId);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException)
            {
                Log($"Method {nameof(this.service.CreateRecord)} finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            Log($"Calling {nameof(this.service.CreateRecord)}() with " +
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

            int newRecordId;
            try
            {
                newRecordId = this.service.Insert(fileCabinetRecord);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException)
            {
                Log($"Method {nameof(this.service.Insert)} finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            Log($"Calling {nameof(this.service.Insert)}() with " +
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
        public void EditRecord(int id, FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            try
            {
                this.service.EditRecord(id, recordParameters);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException)
            {
                Log($"Method {nameof(this.service.EditRecord)} finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            Log($"Calling {nameof(this.service.EditRecord)}() with " +
                $"FirstName = '{recordParameters.FirstName}', " +
                $"LastName = '{recordParameters.LastName}', " +
                $"DateOfBirth = '{recordParameters.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"Wallet = '{recordParameters.Wallet}', " +
                $"MaritalStatus = '{recordParameters.MaritalStatus}', " +
                $"Height = '{recordParameters.Height}'");
        }

        /// <inheritdoc/>
        public List<int> Update(List<KeyValuePair<string, string>> newRecordParameters, List<KeyValuePair<string, string>> searchOptions)
        {
            if (newRecordParameters == null)
            {
                throw new ArgumentNullException(nameof(newRecordParameters));
            }

            if (searchOptions == null)
            {
                throw new ArgumentNullException(nameof(searchOptions));
            }

            var identifiersOfUpdatedRecords = this.service.Update(newRecordParameters, searchOptions);

            Log($"Calling {nameof(this.service.Update)}()");

            return identifiersOfUpdatedRecords;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectByCriteria(SearchProperties searchProperties)
        {
            if (searchProperties == null)
            {
                throw new ArgumentNullException(nameof(searchProperties));
            }

            IEnumerable<FileCabinetRecord> selectedRecords;
            try
            {
                selectedRecords = this.service.SelectByCriteria(searchProperties);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException)
            {
                Log($"Method {nameof(this.service.SelectByCriteria)} finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

            Log($"Calling {nameof(this.service.SelectByCriteria)}() returned {selectedRecords.Count()} record(s)");

            return selectedRecords;
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
        public List<int> Delete(string key, string value)
        {
            List<int> identifiersOfDeletedRecords;
            try
            {
                identifiersOfDeletedRecords = this.service.Delete(key, value);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException)
            {
                Log($"Method {nameof(this.service.Delete)} finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

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

            int numberOfRestoredRecords;
            try
            {
                numberOfRestoredRecords = this.service.Restore(fileCabinetServiceSnapshot);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException)
            {
                Log($"Method {nameof(this.service.Restore)} finished with exception: " +
                    $"Message - {ex.Message}");
                throw;
            }

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
