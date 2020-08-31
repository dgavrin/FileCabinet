using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using FileCabinetApp.Records;

namespace FileCabinetApp.Writers
{
    /// <summary>
    /// File cabinet record xml writer.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="xmlWriter">The xmlWriter.</param>
        public FileCabinetRecordXmlWriter(XmlWriter xmlWriter)
        {
            this.writer = xmlWriter ?? throw new ArgumentNullException(nameof(xmlWriter));
        }

        /// <summary>
        /// Writes a record to a xml file.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="xmlDoc">Xml document.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические", Justification = "<Ожидание>")]
        public void Write(FileCabinetRecord record, XDocument xmlDoc)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (xmlDoc == null)
            {
                throw new ArgumentNullException(nameof(xmlDoc));
            }

            xmlDoc.Root.Add(new XElement(
                "record",
                new XAttribute("Id", record.Id),
                new XElement("name", new XAttribute("first", record.FirstName), new XAttribute("last", record.LastName)),
                new XElement("dateOfBirth", record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)),
                new XElement("wallet", record.Wallet),
                new XElement("maritalStatus", record.MaritalStatus.ToString(CultureInfo.InvariantCulture).ToUpperInvariant()),
                new XElement("height", record.Height)));
        }
    }
}
