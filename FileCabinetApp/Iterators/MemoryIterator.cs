using System;
using System.Collections;
using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Record iterator for FileCabinetMemoryService.
    /// </summary>
    public class MemoryIterator : IEnumerable<FileCabinetRecord>
    {
        private readonly List<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">Records.</param>
        public MemoryIterator(List<FileCabinetRecord> records)
        {
            this.records = records ?? throw new ArgumentNullException(nameof(records));
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            foreach (var record in this.records)
            {
                yield return record;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
