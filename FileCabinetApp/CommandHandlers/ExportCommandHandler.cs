using System;
using System.IO;
using System.Text;
using System.Xml;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Export command handler.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase, ICommandHandler
    {
        private const string Command = "export";

        private ICommandHandler nextHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public ExportCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Export(appCommandRequest.Parameters);
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

        private void Export(string parameters)
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
                                var snapshot = this.fileCabinetService.MakeSnapshot();
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
                                var snapshot = this.fileCabinetService.MakeSnapshot();
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
    }
}
