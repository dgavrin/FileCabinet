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
        /// Creates a record with personal information about a person and adds it to the list.
        /// </summary>
        /// <param name="recordParameters">FileCabinetRecord fields.</param>
        /// <returns>Identifier of the new record.</returns>
        public int CreateRecord(RecordParameters recordParameters);

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
        /// Gets a list of entries by first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>List of entries.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Gets a list of entries by last name.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>List of entries.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Gets a list of entries by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>List of entries.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

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
        /// Deletes the record with the specified ID and returns and returns a value indicating whether the deletion was successful.
        /// </summary>
        /// <param name="recordIdForRemove">The identifier of the entry to be deleted.</param>
        /// <returns>A value used to determine if the deletion was successful.</returns>
        public bool Remove(int recordIdForRemove);
    }
}
