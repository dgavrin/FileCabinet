using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Stat command handler.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const string Command = "stat";

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
                Stat(appCommandRequest.Parameters);
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

        private static void Stat(string parameters)
        {
            var recordsCount = Program.FileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.active} active record(s).");

            if (Program.FileCabinetService is FileCabinetFileSystemService)
            {
                Console.WriteLine($"{recordsCount.removed} removed record(s).");
            }

            Console.WriteLine();
        }
    }
}
