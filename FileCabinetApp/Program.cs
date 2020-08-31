using System;
using System.IO;
using FileCabinetApp.CommandHandlers;
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
        private const string FileName = "cabinet-records.db";

        private static string validationRules;
        private static string storage;
        private static IFileCabinetService fileCabinetService;
        private static bool isRunning = true;
        private static bool isServiceMeterEnable = false;
        private static bool isServiceLoggerEnable = false;

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
                const int parametersIndex = 1;
                var command = inputs[commandIndex];
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var commandHandler = CreateCommandHandlers();
                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
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
                        else if (args[commandIndex].Contains("--use-stopwatch", StringComparison.InvariantCulture))
                        {
                            Program.isServiceMeterEnable = true;
                        }
                        else if (args[commandIndex].Contains("--use-logger", StringComparison.InvariantCulture))
                        {
                            Program.isServiceLoggerEnable = true;
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

            if (isServiceMeterEnable)
            {
                Program.fileCabinetService = new ServiceMeter(Program.fileCabinetService);
            }

            if (isServiceLoggerEnable)
            {
                Program.fileCabinetService = new ServiceLogger(Program.fileCabinetService);
            }
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpCommandHandler = new HelpCommandHandler();
            var exitCommandHandler = new ExitCommandHandler(Program.fileCabinetService, x => isRunning = x);
            var statCommandHandler = new StatCommandHandler(Program.fileCabinetService);
            var createCommandHandler = new CreateCommandHandler(Program.fileCabinetService);
            var selectCommandHandler = new SelectCommandHandler(Program.fileCabinetService);
            var updateCommandHandler = new UpdateCommandHandler(Program.fileCabinetService);
            var exportCommandHandler = new ExportCommandHandler(Program.fileCabinetService);
            var importCommandHandler = new ImportCommandHandler(Program.fileCabinetService);
            var deleteCommandHandler = new DeleteCommandHandler(Program.fileCabinetService);
            var purgeCommandHandler = new PurgeCommandHandler(Program.fileCabinetService);
            var insertCommandHandler = new InsertCommandHandler(Program.fileCabinetService);
            var missingCommandHandler = new MissingCommandHandler();

            helpCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(updateCommandHandler);
            updateCommandHandler.SetNext(selectCommandHandler);
            selectCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(deleteCommandHandler);
            deleteCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(insertCommandHandler);
            insertCommandHandler.SetNext(missingCommandHandler);

            return helpCommandHandler;
        }
    }
}