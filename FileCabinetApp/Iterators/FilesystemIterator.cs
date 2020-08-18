using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Record iterator for FileCabinetFilesystemService.
    /// </summary>
    public class FilesystemIterator : IRecordIterator
    {
        private readonly FileCabinetFileSystemService service;

        private int currentIndex;
        private long[] offsets;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="service">FileCabinetFilesystemService.</param>
        /// <param name="offsets">Offsets.</param>
        public FilesystemIterator(FileCabinetFileSystemService service, IEnumerable<long> offsets)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.offsets = offsets.ToArray() ?? throw new ArgumentNullException(nameof(offsets));
            this.currentIndex = 0;
        }

        /// <inheritdoc/>
        public int Count => this.offsets.Length;

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            FileCabinetRecord nextRecord = null;
            if (this.HasMore() && this.service.TryGetRecordByOffset(this.offsets[this.currentIndex++], ref nextRecord))
            {
                return nextRecord;
            }
            else
            {
                return new FileCabinetRecord();
            }
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return this.currentIndex < this.Count;
        }
    }
}
