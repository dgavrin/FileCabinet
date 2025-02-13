﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FileCabinetApp.Records;
using FileCabinetApp.Validators.InputValidator;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// File cabinet service which displays information about the execution time of the operation.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly Stopwatch watch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public ServiceMeter(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
            this.watch = new Stopwatch();
        }

        /// <inheritdoc/>
        public IInputValidator InputValidator => this.service.InputValidator;

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord recordParameters, bool useId = false)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            this.watch.Reset();
            this.watch.Start();

            var newRecordId = this.service.CreateRecord(recordParameters, useId);

            this.watch.Stop();
            Console.WriteLine($"Create method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();

            return newRecordId;
        }

        /// <inheritdoc/>
        public int Insert(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            this.watch.Reset();
            this.watch.Start();

            var newRecordId = this.service.Insert(fileCabinetRecord);

            this.watch.Stop();
            Console.WriteLine($"Insert method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();

            return newRecordId;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, FileCabinetRecord recordParameters)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            this.watch.Reset();
            this.watch.Start();

            this.service.EditRecord(id, recordParameters);

            this.watch.Stop();
            Console.WriteLine($"Edit method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
        }

        /// <inheritdoc/>
        public List<int> Update(List<KeyValuePair<string, string>> newRecordParameters, List<KeyValuePair<string, string>> searchOptions)
        {
            if (newRecordParameters == null)
            {
                throw new ArgumentNullException(nameof(newRecordParameters));
            }

            if (searchOptions == null)
            {
                throw new ArgumentNullException(nameof(searchOptions));
            }

            this.watch.Reset();
            this.watch.Start();

            var identifiersOfUpdatedRecords = this.service.Update(newRecordParameters, searchOptions);

            this.watch.Stop();
            Console.WriteLine($"Edit method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();

            return identifiersOfUpdatedRecords;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectByCriteria(SearchProperties searchProperties)
        {
            if (searchProperties == null)
            {
                throw new ArgumentNullException(nameof(searchProperties));
            }

            this.watch.Reset();
            this.watch.Start();

            var selectedRecords = this.service.SelectByCriteria(searchProperties);

            this.watch.Stop();
            Console.WriteLine($"Select method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();

            return selectedRecords;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.watch.Reset();
            this.watch.Start();

            var collectionOfReceivedRecods = this.service.GetRecords();

            this.watch.Stop();
            Console.WriteLine($"Get records method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();

            return collectionOfReceivedRecods;
        }

        /// <inheritdoc/>
        public (int active, int removed) GetStat()
        {
            this.watch.Reset();
            this.watch.Start();

            var statistics = this.service.GetStat();

            this.watch.Stop();
            Console.WriteLine($"Get stat method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();

            return statistics;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.watch.Reset();
            this.watch.Start();

            var snapshot = this.service.MakeSnapshot();

            this.watch.Stop();
            Console.WriteLine($"Make snapshot method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();

            return snapshot;
        }

        /// <inheritdoc/>
        public List<int> Delete(string key, string value)
        {
            this.watch.Reset();
            this.watch.Start();

            var identifiersOfDeletedRecords = this.service.Delete(key, value);

            this.watch.Stop();
            Console.WriteLine($"Delete method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();

            return identifiersOfDeletedRecords;
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            if (fileCabinetServiceSnapshot == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetServiceSnapshot));
            }

            this.watch.Reset();
            this.watch.Start();

            var numberOfRestoredRecords = this.service.Restore(fileCabinetServiceSnapshot);

            this.watch.Stop();
            Console.WriteLine($"Restore method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();

            return numberOfRestoredRecords;
        }
    }
}
