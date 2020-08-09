using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using FileCabinetApp.Readers;
using FileCabinetApp.Records;
using FileCabinetApp.Writers;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Represents a snapshot of a file cabinet service.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="list">List of records.</param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            this.records = list.ToArray();
        }

        /// <summary>
        /// Gets returns captured records.
        /// </summary>
        /// <value>Returns captured records.</value>
        public ReadOnlyCollection<FileCabinetRecord> Records
        {
            get { return new ReadOnlyCollection<FileCabinetRecord>(this.records); }
        }

        /// <summary>
        /// Saves a list of entries to a csv file.
        /// </summary>
        /// <param name="streamWriter">Stream for recording.</param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            if (streamWriter == null)
            {
                throw new ArgumentNullException(nameof(streamWriter));
            }

            var writer = new FileCabinetRecordCsvWriter(streamWriter);
            streamWriter.WriteLine("Id, First name, Last name, Date of Birth, Wallet, Marital status, Height");

            foreach (var record in this.records)
            {
                writer.Write(record);
            }
        }

        /// <summary>
        /// Loads from csv.
        /// </summary>
        /// <param name="streamReader">StreamReader.</param>
        public void LoadFromCsv(StreamReader streamReader)
        {
            if (streamReader == null)
            {
                throw new ArgumentNullException(nameof(streamReader));
            }

            var csvReader = new FileCabinetRecordCsvReader(streamReader);
            var loadedRecords = csvReader.ReadAll();

            if (loadedRecords.Count == 0)
            {
                return;
            }

            this.records = loadedRecords.ToArray<FileCabinetRecord>();
        }

        /// <summary>
        /// Loads from xml.
        /// </summary>
        /// <param name="streamReader">StreamReader.</param>
        public void LoadFromXml(StreamReader streamReader)
        {
            if (streamReader == null)
            {
                throw new ArgumentNullException(nameof(streamReader));
            }

            var xmlReader = new FileCabinetRecordXmlReader(streamReader);
            var loadedRecords = xmlReader.ReadAll();

            if (loadedRecords.Count == 0)
            {
                return;
            }

            this.records = loadedRecords.ToArray<FileCabinetRecord>();
        }

        /// <summary>
        /// Saves a list of entries to a xml file.
        /// </summary>
        /// <param name="xmlWriter">Stream fo recording.</param>
        public void SaveToXml(XmlWriter xmlWriter)
        {
            if (xmlWriter == null)
            {
                throw new ArgumentNullException(nameof(xmlWriter));
            }

            var writer = new FileCabinetRecordXmlWriter(xmlWriter);
            var xmlDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("records"));

            foreach (var record in this.records)
            {
                writer.Write(record, xmlDoc);
            }

            xmlDoc.Save(xmlWriter);
        }
    }
}
