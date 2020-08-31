using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Records;
using FileCabinetApp.Validators.InputValidator;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Interface for file cabinet services.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Gets input validator.
        /// </summary>
        /// <value>
        /// Input validator.
        /// </value>
        public IInputValidator InputValidator { get; }

        /// <summary>
        /// Creates a record with personal information about the person and with the specified identifier and adds it to the list.
        /// </summary>
        /// <param name="recordParameters">FileCabinetRecord fields.</param>
        /// <param name="id">Identifier.</param>
        /// <returns>Identifier of the new record.</returns>
        public int CreateRecord(FileCabinetRecord recordParameters, int id = int.MinValue);

        /// <summary>
        /// Inserts a record with personal information about a person into the list.
        /// </summary>
        /// <param name="fileCabinetRecord">FileCabinetRecord.</param>
        /// <returns>The identifier of the inserted record.</returns>
        public int Insert(FileCabinetRecord fileCabinetRecord);

        /// <summary>
        /// Gets a list of entries.
        /// </summary>
        /// <returns>List of entries.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Get statistics about records.
        /// </summary>
        /// <returns>Number of records.</returns>
        public (int active, int removed) GetStat();

        /// <summary>
        /// Edits a record by ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="recordParameters">FileCabinetRecord fields.</param>
        public void EditRecord(int id, RecordParameters recordParameters);

        /// <summary>
        /// Updates a records with specified parameters.
        /// </summary>
        /// <param name="newRecordParameters">A set of new recording parameters.</param>
        /// <param name="searchOptions">A set of record search parameters.</param>
        /// <returns>The list of updated records identifiers.</returns>
        public List<int> Update(List<KeyValuePair<string, string>> newRecordParameters, List<KeyValuePair<string, string>> searchOptions);

        /// <summary>
        /// Returns a selection of records based on the specified criteria.
        /// </summary>
        /// <param name="searchCriteria">Search criteria.</param>
        /// <param name="logicalOperator">Logical operator.</param>
        /// <returns>Selection of records based on specified criteria.</returns>
        public IEnumerable<FileCabinetRecord> SelectByCriteria(List<KeyValuePair<string, string>> searchCriteria, string logicalOperator);

        /// <summary>
        /// Creates a snapshot of a file cabinet service.
        /// </summary>
        /// <returns>A snapshot of the file cabinet service.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores snapshot.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">FileCabinetServiceSnapshot.</param>
        /// <returns>Number of imported records.</returns>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot);

        /// <summary>
        /// Delete record with specified key and value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <returns>The list of deleted records indentifiers.</returns>
        public List<int> Delete(string key, string value);
    }
}
