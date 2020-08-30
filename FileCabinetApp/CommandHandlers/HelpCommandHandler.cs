using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Help command handler.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string Command = "help";

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the statistics by records", "The 'stat' command prints the statistics by records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "export", "exports the list of records to a <csv/xml> file at the specified path", "The 'export' command exports the list of records to a <csv/xml> file at the specified path." },
            new string[] { "import", "imports a list of records from the csv file at the specified path", "The 'import csv' command imports a list of records from the csv file at the specified path." },
            new string[] { "purge", "defragments the data file", "The 'purge' command defragments the data file." },
            new string[] { "insert", "inserts a new record", "The 'insert' command inserts a new record." },
            new string[] { "delete", "deletes entries with the specified key", "The 'delete' command deletes entries with the specified key." },
            new string[] { "update", "updates entries with the specified key", "The 'update' command updates entries with the specified key." },
            new string[] { "select", "gets a selection of records", "The 'select' command gets a selection of records." },
        };

        private ICommandHandler nextHandler;

        /// <summary>
        /// Gets a list of existing commands.
        /// </summary>
        /// <returns>List of existing commands.</returns>
        public static string[] GetListOfExistCommands()
        {
            var existscommnads = new List<string>();

            foreach (var commands in helpMessages)
            {
                existscommnads.Add(commands[0]);
            }

            return existscommnads.ToArray();
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
                PrintHelp(appCommandRequest.Parameters);
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

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
