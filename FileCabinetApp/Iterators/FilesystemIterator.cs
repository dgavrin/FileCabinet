using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Record iterator for FileCabinetFilesystemService.
    /// </summary>
    public class FilesystemIterator : IEnumerable<FileCabinetRecord>
    {
        private readonly FileStream fileStream;
        private readonly List<long> offsets;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="fileStream">FileStream.</param>
        /// <param name="offsets">Offsets.</param>
        public FilesystemIterator(FileStream fileStream, List<long> offsets)
        {
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.offsets = offsets ?? throw new ArgumentNullException(nameof(offsets));
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            foreach (var offset in this.offsets)
            {
                var recordBuffer = new byte[FileCabinetFileSystemService.RecordSize];
                var nextRecord = new FileCabinetRecord();

                this.fileStream.Seek(offset, SeekOrigin.Begin);
                this.fileStream.Read(recordBuffer, 0, recordBuffer.Length);

                _ = FileCabinetFileSystemService.BytesToFileCabinetRecord(recordBuffer, out nextRecord);
                yield return nextRecord;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
