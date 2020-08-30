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
        /// <param name="keyValuePairs">SearchCriteria.</param>
        /// <param name="logicalOperation">Logical operation.</param>
        /// <returns>Enumerable collection of records.</returns>
        public IEnumerable<FileCabinetRecord> Select(List<KeyValuePair<string, string>> keyValuePairs, string logicalOperation)
        {
            var key = GenerateKey(Tuple.Create(keyValuePairs, logicalOperation));
            if (!this.RequestCache.TryGetValue(key, out IEnumerable<FileCabinetRecord> records))
            {
                records = service.SelectByCriteria(keyValuePairs, logicalOperation);
                this.RequestCache.Add(key, records);
            }

            return records;
        }

        private static string GenerateKey(Tuple<List<KeyValuePair<string, string>>, string> rawKey)
        {
            const string separator = "?";

            var generatedKey = new StringBuilder();
            generatedKey.Append(separator);

            foreach (var keyValuePair in rawKey.Item1)
            {
                generatedKey.Append($"{keyValuePair.Key}{keyValuePair.Value}{separator}");
            }

            generatedKey.Append($"{rawKey.Item2}{separator}");

            return generatedKey.ToString();
        }
    }
}
