using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for a non-existent command.
    /// </summary>
    public class PrintMissedCommandHandler : CommandHandlerBase
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
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}
