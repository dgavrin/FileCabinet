using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp
{
    /// <summary>
    /// The memoizer class.
    /// </summary>
    public class Memoizer
    {
        private static readonly Lazy<Memoizer> LazyMemoizer = new Lazy<Memoizer>(() => new Memoizer());
        private static IFileCabinetService service;

        private Memoizer()
        {
            this.RequestCache = new Dictionary<string, IEnumerable<FileCabinetRecord>>();
        }

        /// <summary>
        /// Gets a collection of records for a custom request.
        /// </summary>
        /// <value>
        /// A collection of records for a custom request.
        /// </value>
        public Dictionary<string, IEnumerable<FileCabinetRecord>> RequestCache { get; private set; }

        /// <summary>
        /// Gets the memoizer.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        /// <returns>Memoizer object.</returns>
        public static Memoizer GetMemoizer(IFileCabinetService fileCabinetService)
        {
            service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
            return LazyMemoizer.Value;
        }

        /// <summary>
        /// Clears cached requests.
        /// </summary>
        public void Clear()
        {
            this.RequestCache.Clear();
        }

        /// <summary>
        /// Selects records by the specified key-value pairs.
        /// </summary>
        /// <param name="searchProperties">Search properties.</param>
        /// <returns>Enumerable collection of records.</returns>
        public IEnumerable<FileCabinetRecord> Select(SearchProperties searchProperties)
        {
            if (searchProperties is null)
            {
                throw new ArgumentNullException(nameof(searchProperties));
            }

            var key = GenerateKey(searchProperties);
            if (!this.RequestCache.TryGetValue(key, out IEnumerable<FileCabinetRecord> records))
            {
                records = service.SelectByCriteria(searchProperties);
                this.RequestCache.Add(key, records);
            }

            return records;
        }

        private static string GenerateKey(SearchProperties rawKey)
        {
            const string separator = "?";

            var generatedKey = new StringBuilder();
            generatedKey.Append(separator);

            for (int currentPropertyIndex = 0; currentPropertyIndex < rawKey.List.Count - 1; currentPropertyIndex++)
            {
                generatedKey.Append($"{rawKey.List[currentPropertyIndex]}={rawKey.List[currentPropertyIndex + 1]}{separator}{rawKey.Signs[currentPropertyIndex]}{separator}");
            }

            return generatedKey.ToString();
        }
    }
}
