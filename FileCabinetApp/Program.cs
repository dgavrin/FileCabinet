using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using FileCabinetApp.Records;
using FileCabinetApp.Services;

namespace FileCabinetApp
{
    /// <summary>
    /// The main program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Denis Gavrin";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string FileName = "cabinet-records.db";

        private static bool isRunning = true;
        private static string validationRules;
        private static string storage;
        private static IFileCabinetService fileCabinetService;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the statistics by records", "The 'stat' command prints the statistics by records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "returns a list of records added to the service", "The 'list' command returns a list of records added to the service." },
            new string[] { "edit", "edits a record", "The 'edit' command edits a record." },
            new string[] { "find", "finds records for the specified key", "The 'find' command finds records for the specified key" },
            new string[] { "export", "exports the list of records to a <csv/xml> file at the specified path", "The 'export' command exports the list of records to a <csv/xml> file at the specified path" },
            new string[] { "import", "imports a list of records from the csv file at the specified path", "The 'import csv' command imports a list of records from the csv file at the specified path" },
            new string[] { "remove", "removal record by id", "The 'remove' command removes a record by id." },
            new string[] { "purge", "defragments the data file", "The 'purge' command defragments the data file." },
        };

        /// <summary>
        /// The main method.
        /// </summary>
        public static void Main()
        {
            GetApplicationSettings();
            CreateFileCabinetService();

#pragma warning disable CA1308
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {validationRules.ToLowerInvariant()} validation rules.");
            Console.WriteLine($"Using {storage.ToLowerInvariant()} as data store.");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();
#pragma warning restore CA1308

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;

            if (Program.fileCabinetService is FileCabinetFileSystemService service)
            {
                service.Dispose();
            }
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.active} active record(s).");

            if (Program.fileCabinetService is FileCabinetFileSystemService)
            {
                Console.WriteLine($"{recordsCount.removed} removed record(s).");
            }

            Console.WriteLine();
        }

        private static void Create(string parameters)
        {
            bool invalidValues = true;

            do
            {
                try
                {
                    var newRecord = fileCabinetService.SetInformationToRecord();
                    var recordId = Program.fileCabinetService.CreateRecord(newRecord);
                    Console.WriteLine($"Record #{recordId} is created.");
                    Console.WriteLine();

                    invalidValues = false;
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine($"Please try again. {ex.Message}");
                    Console.WriteLine();
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Please try again. {ex.Message}");
                    Console.WriteLine();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please try again and enter valid data.");
                    Console.WriteLine();
                }
            }
            while (invalidValues);
        }

        private static void List(string parameters)
        {
            var listOfRecords = Program.fileCabinetService.GetRecords();

            fileCabinetService.DisplayRecords(listOfRecords);
            Console.WriteLine();
        }

        private static void Edit(string parameters)
        {
            if (parameters.Length == 0)
            {
                Console.WriteLine("Please try again. Enter record ID. 'edit <ID>'.");
                Console.WriteLine();
                return;
            }

            var recordIdForEdit = Convert.ToInt32(parameters, CultureInfo.InvariantCulture);
            var listOfRecords = Program.fileCabinetService.GetRecords();

            foreach (var record in listOfRecords)
            {
                if (record.Id == recordIdForEdit)
                {
                    try
                    {
                        var editRecord = fileCabinetService.SetInformationToRecord();
                        Program.fileCabinetService.EditRecord(recordIdForEdit, editRecord);
                        Console.WriteLine($"Record #{recordIdForEdit} is updated.");
                        Console.WriteLine();
                        return;
                    }
                    catch (ArgumentNullException ex)
                    {
                        Console.WriteLine($"Please try again. {ex.Message}");
                        Console.WriteLine();
                        return;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Please try again. {ex.Message}");
                        Console.WriteLine();
                        return;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Please try again and enter valid data.");
                        Console.WriteLine();
                        return;
                    }
                }
            }

            Console.WriteLine($"#{recordIdForEdit} record is not found.");
            Console.WriteLine();
        }

        private static void Find(string parameters)
        {
            Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[] searchCommands = new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[]
            {
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("firstname", Program.fileCabinetService.FindByFirstName),
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("lastname", Program.fileCabinetService.FindByLastName),
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("dateofbirth", Program.fileCabinetService.FindByDateOfBirth),
            };

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] inputParameters = parameters.Split(' ', 2);

                if (inputParameters.Length < 2)
                {
                    Console.WriteLine("Please try again. Enter the key. The syntax for the 'find' command is \"find <search by> <key> \".");
                    Console.WriteLine();
                    return;
                }

                const int commandIndex = 0;
                const int argumentIndex = 1;
                var command = inputParameters[commandIndex];
                var argument = inputParameters[argumentIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine($"Please try again. The '{command}' is invalid parameter.");
                    Console.WriteLine();
                    return;
                }

                var index = Array.FindIndex(searchCommands, 0, searchCommands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    var foundRecords = searchCommands[index].Item2(argument);

                    if (foundRecords.Count == 0)
                    {
                        Console.WriteLine($"There are no entries with parameter '{argument}'.");
                    }
                    else
                    {
                        fileCabinetService.DisplayRecords(foundRecords);
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine($"Search by {command} is not possible.");
                }
            }
            else
            {
                Console.WriteLine("Error entering parameters. The syntax for the 'find' command is \"find <search by> <key> \".");
                Console.WriteLine();
            }
        }

        private static void GetApplicationSettings()
        {
            const int parameter = 1;
            Program.validationRules = "default";
            Program.storage = "memory";
            var args = Environment.GetCommandLineArgs()[1..];

            if (args.Length > 0)
            {
                for (int commandIndex = 0; commandIndex < args.Length; commandIndex++)
                {
                    if (args[commandIndex].Contains('-', StringComparison.InvariantCulture))
                    {
                        if (args[commandIndex].Equals("-v", StringComparison.InvariantCulture) && commandIndex + 1 < args.Length)
                        {
                            Program.validationRules = args[commandIndex + 1];
                        }
                        else if (args[commandIndex].Contains("--validation-rules=", StringComparison.InvariantCulture))
                        {
                            Program.validationRules = args[commandIndex].Split('=')[parameter];
                        }
                        else if (args[commandIndex].Equals("-s", StringComparison.InvariantCulture) && commandIndex + 1 < args.Length)
                        {
                            Program.storage = args[commandIndex + 1];
                        }
                        else if (args[commandIndex].Contains("--storage=", StringComparison.InvariantCulture))
                        {
                            Program.storage = args[commandIndex].Split('=')[parameter];
                        }
                    }
                }
            }

            if (!Program.validationRules.Equals("default", StringComparison.InvariantCultureIgnoreCase) && !Program.validationRules.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
            {
                Program.validationRules = "default";
            }

            if (!Program.storage.Equals("memory", StringComparison.InvariantCultureIgnoreCase) && !Program.storage.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                Program.storage = "memory";
            }
        }

        private static void CreateFileCabinetService()
        {
            switch (Program.storage)
            {
                case "file":
                    Program.fileCabinetService = new FileCabinetFileSystemService(new FileStream(Program.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), validationRules);
                    break;
                default:
                    Program.fileCabinetService = new FileCabinetMemoryService(Program.validationRules);
                    break;
            }
        }

        private static void Export(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                string[] inputParameters = parameters.Split(' ', 2);

                if (inputParameters.Length < 2)
                {
                    Console.WriteLine("Please try again. Enter the key. The syntax for the 'export' command is \"export csv <fileName> \".");
                    Console.WriteLine();
                    return;
                }

                const int commandIndex = 0;
                const int fileNameIndex = 1;
                var command = inputParameters[commandIndex];
                var fileName = inputParameters[fileNameIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine($"Please try again. The '{command}' is invalid parameter.");
                    Console.WriteLine();
                    return;
                }

                if (File.Exists(fileName))
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
                    if (command.ToUpperInvariant() == "CSV")
                    {
                        if (fileName.EndsWith(".csv", StringComparison.InvariantCulture))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(fileName))
                            {
                                var snapshot = fileCabinetService.MakeSnapshot();
                                snapshot.SaveToCsv(streamWriter);
                                ReportExportSuccess(fileName);
                            }
                        }
                        else
                        {
                            ReportAFileExtensionError();
                        }
                    }
                    else if (command.ToUpperInvariant() == "XML")
                    {
                        if (fileName.EndsWith(".xml", StringComparison.InvariantCulture))
                        {
                            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                            xmlWriterSettings.Encoding = Encoding.UTF8;
                            xmlWriterSettings.Indent = true;
                            xmlWriterSettings.IndentChars = "\t";

                            using (XmlWriter xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings))
                            {
                                var snapshot = fileCabinetService.MakeSnapshot();
                                snapshot.SaveToXml(xmlWriter);
                                ReportExportSuccess(fileName);
                            }
                        }
                        else
                        {
                            ReportAFileExtensionError();
                        }
                    }
                    else
                    {
                        ReportAnErrorWhileEnteringParameters();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    ReportAnExportError(fileName);
                }
                catch (IOException)
                {
                    ReportAnExportError(fileName);
                }
            }
            else
            {
                ReportAnErrorWhileEnteringParameters();
            }

            void ReportAnExportError(string path)
            {
                Console.WriteLine($"Export failed: can't open file {path}.");
                Console.WriteLine();
            }

            void ReportExportSuccess(string path)
            {
                Console.WriteLine($"All records are exported to file {path}.");
                Console.WriteLine();
            }

            void ReportAFileExtensionError()
            {
                Console.WriteLine("When using \"export\", the type of the <csv/xml> command and the file extension must match.");
                Console.WriteLine();
            }

            void ReportAnErrorWhileEnteringParameters()
            {
                Console.WriteLine("Error entering parameters. The syntax for the 'export' command is \"export <csv/xml> <fileName>\".");
                Console.WriteLine();
            }
        }

        private static void Import(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                string[] inputParameters = parameters.Split(' ', 2);

                if (inputParameters.Length < 2)
                {
                    Console.WriteLine("Please try again. Enter the key. The syntax for the 'import' command is \"import csv <fileName> \".");
                    Console.WriteLine();
                    return;
                }

                const int commandIndex = 0;
                const int fileNameIndex = 1;
                var command = inputParameters[commandIndex];
                var fileName = inputParameters[fileNameIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine($"Please try again. The '{command}' is invalid parameter.");
                    Console.WriteLine();
                    return;
                }

                if (command.Equals("csv", StringComparison.InvariantCultureIgnoreCase) || command.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!File.Exists(fileName))
                    {
                        Console.WriteLine($"Import error: file {fileName} is not exist.");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        if (command.ToUpperInvariant() == "CSV")
                        {
                            if (fileName.EndsWith(".csv", StringComparison.InvariantCulture))
                            {
                                using (StreamReader streamReader = new StreamReader(fileName))
                                {
                                    Console.WriteLine("Please wait. Importing records may take some time.");
                                    var fileCabinetServiceSnapshot = Program.fileCabinetService.MakeSnapshot();
                                    fileCabinetServiceSnapshot.LoadFromCsv(streamReader);
                                    var importedRecordsCount = Program.fileCabinetService.Restore(fileCabinetServiceSnapshot);
                                    Console.WriteLine($"{importedRecordsCount} records were imported from {fileName}.");
                                    Console.WriteLine();
                                }
                            }
                            else
                            {
                                ReportAFileExtensionError();
                            }
                        }

                        if (command.ToUpperInvariant() == "XML")
                        {
                            if (fileName.EndsWith(".xml", StringComparison.InvariantCulture))
                            {
                                using (StreamReader streamReader = new StreamReader(fileName))
                                {
                                    Console.WriteLine("Please wait. Importing records may take some time.");
                                    var fileCabinetServiceSnapshot = Program.fileCabinetService.MakeSnapshot();
                                    fileCabinetServiceSnapshot.LoadFromXml(streamReader);
                                    var importedRecordsCount = Program.fileCabinetService.Restore(fileCabinetServiceSnapshot);
                                    Console.WriteLine($"{importedRecordsCount} records were imported from {fileName}.");
                                    Console.WriteLine();
                                }
                            }
                            else
                            {
                                ReportAFileExtensionError();
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Export failed: can't open file {fileName}.");
                        Console.WriteLine();
                    }
                    catch (IOException)
                    {
                        Console.WriteLine($"Export failed: can't open file {fileName}.");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("When using \"import\", the type of the csv command and the file extension must match.");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Error entering parameters. The syntax for the 'import' command is \"import csv <fileName>\".");
                Console.WriteLine();
            }

            void ReportAFileExtensionError()
            {
                Console.WriteLine("When using \"import\", the type of the <csv/xml> command and the file extension must match.");
                Console.WriteLine();
            }
        }

        private static void Remove(string parameters)
        {
            if (parameters.Length == 0)
            {
                Console.WriteLine("Please try again. Enter record ID. 'remove <ID>'.");
                Console.WriteLine();
                return;
            }

            var recordIdForRemove = Convert.ToInt32(parameters, CultureInfo.InvariantCulture);

            if (Program.fileCabinetService.Remove(recordIdForRemove))
            {
                Console.WriteLine($"Record #{recordIdForRemove} is removed.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"Record #{recordIdForRemove} doesn't exists.");
                Console.WriteLine();
            }
        }

        private static void Purge(string parameters)
        {
            if (Program.fileCabinetService is FileCabinetFileSystemService fileCabinetFileSystemService)
            {
                fileCabinetFileSystemService.Purge();
            }
        }
    }
}