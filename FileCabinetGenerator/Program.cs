using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetGenerator
{
    /// <summary>
    /// The main program class of <see cref="global::FileCabinetGenerator"/>.
    /// </summary>
    public static class Program
    {
        private const string HintMessage = "Enter valid parameters: output-type, output file name, records-amount and start-id.";

        private static readonly Dictionary<string, Action<string>> CommandLineParameters = new Dictionary<string, Action<string>>
        {
            ["--OUTPUT-TYPE"] = new Action<string>(SetOutputType),
            ["-T"] = new Action<string>(SetOutputType),
            ["--OUTPUT"] = new Action<string>(SetOutput),
            ["-O"] = new Action<string>(SetOutput),
            ["--RECORDS-AMOUNT"] = new Action<string>(SetRecordsAmount),
            ["-A"] = new Action<string>(SetRecordsAmount),
            ["--START-ID"] = new Action<string>(SetStartId),
            ["-I"] = new Action<string>(SetStartId),
        };

        private static string outputType = "invalid";
        private static string fileName = "invalid";
        private static int recordsAmount = -3;
        private static int startId = -3;
        private static RecordsGenerator recordsGenerator;

        /// <summary>
        /// The main method of <see cref="global::FileCabinetGenerator"/>.
        /// </summary>
        public static void Main()
        {
            var commandLineArguments = Environment.GetCommandLineArgs()[1..];

            if (GetApplicationPreferences(commandLineArguments))
            {
                DisplayPreferences();

                Program.recordsGenerator = new RecordsGenerator(Program.startId, Program.recordsAmount);
                Program.recordsGenerator.GenerateRecords();

                Export();
            }
            else
            {
                Console.WriteLine(HintMessage);
            }

            Console.WriteLine("Exiting an application...");
            Console.ReadKey();
        }

        private static bool GetApplicationPreferences(string[] commnandLineArguments)
        {
            if (commnandLineArguments == null || commnandLineArguments.Length < 4)
            {
                return false;
            }

            var temporaryArgument = new string[2];

            for (int i = 1; i <= commnandLineArguments.Length;)
            {
                temporaryArgument[0] = commnandLineArguments[i - 1];
                if (i != commnandLineArguments.Length)
                {
                    temporaryArgument[1] = commnandLineArguments[i];
                }

                var parsedCommand = CommandLineParameterParser(temporaryArgument);
                switch (parsedCommand)
                {
                    case TypeOfParameter.NotParameter:
                    case TypeOfParameter.FullParameter:
                        i++;
                        break;
                    case TypeOfParameter.ShortParameter:
                        i += 2;
                        break;
                }
            }

            return CheckPreferences();

            bool CheckPreferences()
            {
                var isValidPreferences = true;

                // Check output type.
                if (Program.outputType.Equals("invalid", StringComparison.InvariantCultureIgnoreCase))
                {
                    isValidPreferences = false;
                    Console.WriteLine("Invalid output type. Should be csv or xml.");
                }

                // Check output file name.
                if (Program.fileName.Equals("invalid", StringComparison.InvariantCultureIgnoreCase))
                {
                    isValidPreferences = false;
                }
                else if (Program.fileName.Equals("invalid output type", StringComparison.InvariantCultureIgnoreCase))
                {
                    isValidPreferences = false;
                }
                else if (Program.fileName.Equals("invalid file extension", StringComparison.InvariantCultureIgnoreCase))
                {
                    isValidPreferences = false;
                    Console.WriteLine("Invalid output file name. The file extension must match the type of output.");
                }

                // Check records amount.
                switch (Program.recordsAmount)
                {
                    case -1:
                        isValidPreferences = false;
                        Console.WriteLine("Invalid records amount. Should be more than 0.");
                        break;
                    case -2:
                        isValidPreferences = false;
                        Console.WriteLine("Invalid records amount. Should be integer.");
                        break;
                    case -3:
                        isValidPreferences = false;
                        Console.WriteLine("Records amount must be set.");
                        break;
                }

                // Check start id.
                switch (Program.startId)
                {
                    case -1:
                        isValidPreferences = false;
                        Console.WriteLine("Invalid start id. Should be more than 0.");
                        break;
                    case -2:
                        isValidPreferences = false;
                        Console.WriteLine("Invalid start id. Should be integer.");
                        break;
                    case -3:
                        isValidPreferences = false;
                        Console.WriteLine("Start id must be set.");
                        break;
                }

                return isValidPreferences;
            }
        }

        private static TypeOfParameter CommandLineParameterParser(string[] temporaryArgument)
        {
            var operation = string.Empty;
            var parameter = string.Empty;
            Action<string> changeApplicaionSetting = null;
            TypeOfParameter parsedCommand = TypeOfParameter.NotParameter;

            if (temporaryArgument[0].StartsWith("--", StringComparison.InvariantCulture))
            {
                var equalSignIndex = temporaryArgument[0].IndexOf("=", StringComparison.InvariantCulture);
                if (equalSignIndex > 0)
                {
                    operation = temporaryArgument[0].Substring(0, equalSignIndex);
                    parameter = temporaryArgument[0].Substring(equalSignIndex + 1);

                    if (CommandLineParameters.ContainsKey(operation.ToUpperInvariant()))
                    {
                        changeApplicaionSetting = CommandLineParameters[operation.ToUpperInvariant()];
                        parsedCommand = TypeOfParameter.FullParameter;
                    }
                }
            }
            else if (temporaryArgument[0].StartsWith("-", StringComparison.InvariantCulture))
            {
                operation = temporaryArgument[0];

                if (temporaryArgument[1] != null)
                {
                    parameter = temporaryArgument[1];
                }

                if (CommandLineParameters.ContainsKey(operation.ToUpperInvariant()))
                {
                    changeApplicaionSetting = CommandLineParameters[operation.ToUpperInvariant()];
                    parsedCommand = TypeOfParameter.ShortParameter;
                }
            }

            if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(parameter) && changeApplicaionSetting != null)
            {
#pragma warning disable CA1308
                changeApplicaionSetting.Invoke(parameter.ToLowerInvariant());
#pragma warning restore CA1308
            }

            return parsedCommand;
        }

        private static void SetOutputType(string outputType)
        {
            if (string.IsNullOrEmpty(outputType))
            {
                throw new ArgumentNullException(nameof(outputType));
            }

            if (outputType.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
            {
                Program.outputType = "csv";
            }
            else if (outputType.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                Program.outputType = "xml";
            }
        }

        private static void SetOutput(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!Program.outputType.Equals("invalid", StringComparison.InvariantCultureIgnoreCase))
            {
                if (fileName.EndsWith("." + outputType, StringComparison.InvariantCulture))
                {
                    Program.fileName = fileName;
                }
                else
                {
                    Program.fileName = "invalid file extension";
                }
            }
            else
            {
                Program.fileName = "invalid output type";
            }
        }

        private static void SetRecordsAmount(string recordsAmount)
        {
            if (string.IsNullOrEmpty(recordsAmount))
            {
                throw new ArgumentNullException(nameof(recordsAmount));
            }

            if (int.TryParse(recordsAmount, out int parsedRecordsAmount))
            {
                if (parsedRecordsAmount > 0)
                {
                    Program.recordsAmount = parsedRecordsAmount;
                }
                else
                {
                    Program.recordsAmount = -1; // Invalid records amount. Should be more than 0.
                }
            }
            else
            {
                Program.recordsAmount = -2; // Invalid records amount. Should be integer.
            }
        }

        private static void SetStartId(string startId)
        {
            if (string.IsNullOrEmpty(startId))
            {
                throw new ArgumentNullException(nameof(startId));
            }

            if (int.TryParse(startId, out int parsedStartId))
            {
                if (parsedStartId > 0)
                {
                    Program.startId = parsedStartId;
                }
                else
                {
                    Program.startId = -1; // Invalid start id. Should be more than 0.
                }
            }
            else
            {
                Program.startId = -2; // Invalid start id. Should be integer.
            }
        }

        private static void DisplayPreferences()
        {
            Console.WriteLine("Select Preferences:");
            Console.WriteLine($"Output-type: {Program.outputType}");
            Console.WriteLine($"Output: {Program.fileName}");
            Console.WriteLine($"Records-amount: {Program.recordsAmount}");
            Console.WriteLine($"Start-id: {Program.startId}");
            Console.WriteLine();
        }

        private static void Export()
        {
            if (File.Exists(Program.fileName))
            {
                Console.Write($"File is exist - rewrite {fileName}? [Y/n]: ");
                char userResponse;
                do
                {
                    userResponse = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                }
                while (userResponse != 'Y' && userResponse != 'y' && userResponse != 'N' && userResponse != 'n');

                if (userResponse == 'n')
                {
                    return;
                }
            }

            try
            {
                switch (Program.outputType)
                {
                    case "csv":
                        using (StreamWriter streamWriter = new StreamWriter(fileName))
                        {
                            var snapshot = Program.recordsGenerator.MakeSnapshot();
                            snapshot.SaveToCsv(streamWriter);
                            ReportExportSuccess(fileName);
                        }

                        break;
                    case "xml":
                        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                        xmlWriterSettings.Encoding = Encoding.UTF8;
                        xmlWriterSettings.Indent = true;
                        xmlWriterSettings.IndentChars = "\t";

                        using (XmlWriter xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings))
                        {
                            var snapshot = Program.recordsGenerator.MakeSnapshot();
                            snapshot.SaveToXml(xmlWriter);
                            ReportExportSuccess(fileName);
                        }

                        break;
                }
            }
            catch (UnauthorizedAccessException)
            {
                ReportAnExportError(Program.fileName);
            }
            catch (IOException)
            {
                ReportAnExportError(Program.fileName);
            }

            void ReportExportSuccess(string path)
            {
                Console.WriteLine($"All records are exported to file {path}.");
                Console.WriteLine();
            }

            void ReportAnExportError(string path)
            {
                Console.WriteLine($"Export failed: can't open file {path}.");
                Console.WriteLine();
            }
        }
    }
}
