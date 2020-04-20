using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
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
        /// <param name="list"> List of records. </param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            this.records = list.ToArray();
        }

        /// <summary>
        /// Saves a list of entries to a csv file.
        /// </summary>
        /// <param name="streamWriter"> Stream for recording. </param>
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
        /// Saves a list of entries to a xml file.
        /// </summary>
        /// <param name="xmlWriter"> Stream fo recording.</param>
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
