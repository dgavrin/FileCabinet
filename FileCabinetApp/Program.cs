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
        public static IFileCabinetService FileCabinetService;
        public static bool IsRunning = true;

        private const string DeveloperName = "Denis Gavrin";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string FileName = "cabinet-records.db";

        private static string validationRules;
        private static string storage;

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
            while (IsRunning);
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
                    Program.FileCabinetService = new FileCabinetFileSystemService(new FileStream(Program.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), validationRules);
                    break;
                default:
                    Program.FileCabinetService = new FileCabinetMemoryService(Program.validationRules);
                    break;
            }
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpCommandHandler = new HelpCommandHandler();
            var exitCommandHandler = new ExitCommandHandler();
            var statCommandHandler = new StatCommandHandler();
            var createCommandHandler = new CreateCommandHandler();
            var listCommandHandler = new ListCommandHandler();
            var editCommandHandler = new EditCommandHandler();
            var findCommandHandler = new FindCommandHandler();
            var exportCommandHandler = new ExportCommandHandler();
            var importCommandHandler = new ImportCommandHandler();
            var removeCommandHandler = new RemoveCommandHandler();
            var purgeCommandHandler = new PurgeCommandHandler();
            var printMissedCommandHandler = new PrintMissedCommandHandler();

            helpCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(listCommandHandler);
            listCommandHandler.SetNext(editCommandHandler);
            editCommandHandler.SetNext(findCommandHandler);
            findCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(removeCommandHandler);
            removeCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(printMissedCommandHandler);

            return helpCommandHandler;
        }
    }
}