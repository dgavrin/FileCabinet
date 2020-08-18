using System;
using System.Collections;
using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Record iterator for FileCabinetMemoryService.
    /// </summary>
    public class MemoryIterator : IEnumerable<FileCabinetRecord>, IEnumerator<FileCabinetRecord>
    {
        private readonly List<FileCabinetRecord> records;

        private int position = 0;
        private bool disposed = false;
        private FileCabinetRecord current;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">Records.</param>
        public MemoryIterator(List<FileCabinetRecord> records)
        {
            this.records = records ?? throw new ArgumentNullException(nameof(records));
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
            if (this.records.Count == 0)
            {
                return false;
            }

            if (this.position < this.records.Count)
            {
                this.current = this.records[this.position++];
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
