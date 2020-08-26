using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for a non-existent command.
    /// </summary>
    public class MissingCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            PrintMissedCommandInfo(appCommandRequest.Command);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            const string similarCommandsMessage = "The most similar commands ";

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var similarCommands = new List<string>();
            foreach (var existCommand in HelpCommandHandler.GetListOfExistCommands())
            {
                if (LevenshteinDistance(command, existCommand) < 3)
                {
                    similarCommands.Add(existCommand);
                }
            }

            Console.WriteLine($"FileCabinetApp: '{command}' is not a FileCabinetApp command. See 'help'.");
            Console.WriteLine();
            if (similarCommands.Count == 1)
            {
                Console.Write(similarCommandsMessage);
                Console.WriteLine("is");
            }
            else if (similarCommands.Count > 1)
            {
                Console.Write(similarCommandsMessage);
                Console.WriteLine("are");
            }
            else
            {
                return;
            }

            similarCommands.ForEach(command => Console.WriteLine($"\t\t{command}"));
            Console.WriteLine();
        }

        private static int MinimalInt(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;

        private static int LevenshteinDistance(string firstString, int firstLenght, string secondString, int secondLenght)
        {
            if (firstLenght == 0)
            {
                return secondLenght;
            }

            if (secondLenght == 0)
            {
                return firstLenght;
            }

            var substitutionCost = 0;
            if (firstString[firstLenght - 1] != secondString[secondLenght - 1])
            {
                substitutionCost = 1;
            }

            var deletion = LevenshteinDistance(firstString, firstLenght - 1, secondString, secondLenght) + 1;
            var insertion = LevenshteinDistance(firstString, firstLenght, secondString, secondLenght - 1) + 1;
            var substitution = LevenshteinDistance(firstString, firstLenght - 1, secondString, secondLenght - 1) + substitutionCost;

            return MinimalInt(deletion, insertion, substitution);
        }

        private static int LevenshteinDistance(string firstString, string secondString) => LevenshteinDistance(firstString, firstString.Length, secondString, secondString.Length);
    }
}
