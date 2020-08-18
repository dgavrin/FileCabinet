using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Record iterator interface.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Gets the number of records.
        /// </summary>
        /// <value>
        /// The number of records.
        /// </value>
        public int Count { get; }

        /// <summary>
        /// Gets the next item in the collection.
        /// </summary>
        /// <returns>The next item in the collection.</returns>
        public FileCabinetRecord GetNext();

        /// <summary>
        /// Displays the presence of the next item in the collection.
        /// </summary>
        /// <returns>Returns true if there is another item in the collection.</returns>
        public bool HasMore();
    }
}
