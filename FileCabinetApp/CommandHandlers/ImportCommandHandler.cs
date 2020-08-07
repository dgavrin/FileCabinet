using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Import command handler.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const string Command = "import";

        private ICommandHandler nextHandler;

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
            {
                Import(appCommandRequest.Parameters);
            }
            else
            {
                this.nextHandler.Handle(appCommandRequest);
            }
        }

        /// <inheritdoc/>
        public new void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
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
                                    var fileCabinetServiceSnapshot = Program.FileCabinetService.MakeSnapshot();
                                    fileCabinetServiceSnapshot.LoadFromCsv(streamReader);
                                    var importedRecordsCount = Program.FileCabinetService.Restore(fileCabinetServiceSnapshot);
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
                                    var fileCabinetServiceSnapshot = Program.FileCabinetService.MakeSnapshot();
                                    fileCabinetServiceSnapshot.LoadFromXml(streamReader);
                                    var importedRecordsCount = Program.FileCabinetService.Restore(fileCabinetServiceSnapshot);
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
    }
}
