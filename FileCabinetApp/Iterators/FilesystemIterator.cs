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
    public class FilesystemIterator : IEnumerable<FileCabinetRecord>, IEnumerator<FileCabinetRecord>
    {
        private readonly FileStream fileStream;
        private readonly List<long> offsets;

        private int position = 0;
        private bool disposed = false;
        private FileCabinetRecord current;

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
        public FileCabinetRecord Current => this.current;

        /// <inheritdoc/>
        object IEnumerator.Current => this.Current;

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (this.offsets.Count == 0)
            {
                return false;
            }

            if (this.position < this.offsets.Count)
            {
                var recordBuffer = new byte[FileCabinetFileSystemService.RecordSize];
                var nextRecord = new FileCabinetRecord();

                this.fileStream.Seek(this.offsets[this.position++], SeekOrigin.Begin);
                this.fileStream.Read(recordBuffer, 0, recordBuffer.Length);

                if (FileCabinetFileSystemService.BytesToFileCabinetRecord(recordBuffer, out nextRecord))
                {
                    this.current = nextRecord;
                }
            }
            else
            {
                this.current = null;
            }

            if (this.current == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.position = 0;
            this.current = null;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "<Ожидание>")]
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Reset();
            }

            this.disposed = true;
        }
    }
}
