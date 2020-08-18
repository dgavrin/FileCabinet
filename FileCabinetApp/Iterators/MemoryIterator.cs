using System;
using System.Collections.Generic;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Record iterator for FileCabinetMemoryService.
    /// </summary>
    public class MemoryIterator : IRecordIterator
    {
        private readonly List<FileCabinetRecord> list;

        private int currentIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">FileCabinetMemoryService.</param>
        public MemoryIterator(IEnumerable<FileCabinetRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            this.list = new List<FileCabinetRecord>(records);
            this.currentIndex = 0;
        }

        /// <inheritdoc/>
        public int Count => this.list.Count;

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            return this.HasMore() ?
                this.list[this.currentIndex++] :
                new FileCabinetRecord();
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return this.currentIndex < this.Count;
        }
    }
}
