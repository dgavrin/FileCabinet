using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using FileCabinetApp.Records;

namespace FileCabinetApp.Readers
{
    /// <summary>
    /// File cabinet records xml reader.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="streamReader">StreamReader.</param>
        public FileCabinetRecordXmlReader(StreamReader streamReader)
        {
            this.reader = streamReader;
        }

        /// <summary>
        /// Gets a list of records to import.
        /// </summary>
        /// <returns>List of records to import.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> readRecords = new List<FileCabinetRecord>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.reader);
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("record");

            foreach (XmlNode node in nodeList)
            {
                readRecords.Add(ParseNode(node));
            }

            return readRecords;

            FileCabinetRecord ParseNode(XmlNode node)
            {
                CultureInfo provider = new CultureInfo("en-US");

                FileCabinetRecord parsedRecord = new FileCabinetRecord()
                {
                    Id = Convert.ToInt32(node.Attributes["Id"].InnerText, provider),
                    FirstName = node.SelectSingleNode("name").Attributes["first"].InnerText,
                    LastName = node.SelectSingleNode("name").Attributes["last"].InnerText,
                    DateOfBirth = DateTime.Parse(node.SelectSingleNode("dateOfBirth").InnerText, provider),
                    Wallet = Convert.ToDecimal(node.SelectSingleNode("wallet").InnerText, provider),
                    MaritalStatus = Convert.ToChar(node.SelectSingleNode("maritalStatus").InnerText, provider),
                    Height = Convert.ToInt16(node.SelectSingleNode("height").InnerText, provider),
                };

                return parsedRecord;
            }
        }
    }
}
