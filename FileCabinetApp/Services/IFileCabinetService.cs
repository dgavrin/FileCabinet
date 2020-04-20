using System.Collections.ObjectModel;
using FileCabinetApp.Records;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Interface for file cabinet services.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates a record with personal information about a person and adds it to the list.
        /// </summary>
        /// <param name="recordParameters"> FileCabinetRecord fields. </param>
        /// <returns> Identifier of the new record. </returns>
        public int CreateRecord(RecordParameters recordParameters);

        /// <summary>
        /// Gets a list of entries.
        /// </summary>
        /// <returns> List of entries. </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Get statistics about records.
        /// </summary>
        /// <returns> Number of records. </returns>
        public int GetStat();

        /// <summary>
        /// Edits a record by ID.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <param name="recordParameters"> FileCabinetRecord fields. </param>
        public void EditRecord(int id, RecordParameters recordParameters);

        /// <summary>
        /// Gets a list of entries by first name.
        /// </summary>
        /// <param name="firstName"> The first name. </param>
        /// <returns> List of entries. </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Gets a list of entries by last name.
        /// </summary>
        /// <param name="lastName"> The last name. </param>
        /// <returns> List of entries. </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Gets a list of entries by date of birth.
        /// </summary>
        /// <param name="dateOfBirth"> The date of birth. </param>
        /// <returns> List of entries. </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        /// <summary>
        /// Enter personal information about the person to record.
        /// </summary>
        /// <returns> RecordParameters. </returns>
        public RecordParameters SetInformationToRecord();
    }
}
