using System;
using System.Collections.ObjectModel;
using FileCabinetApp.Records;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Find command handler.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const string Command = "find";

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
                Find(appCommandRequest.Parameters);
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

        private static void Find(string parameters)
        {
            Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[] searchCommands = new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[]
            {
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("firstname", Program.FileCabinetService.FindByFirstName),
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("lastname", Program.FileCabinetService.FindByLastName),
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("dateofbirth", Program.FileCabinetService.FindByDateOfBirth),
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
                        Program.FileCabinetService.DisplayRecords(foundRecords);
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
    }
}
