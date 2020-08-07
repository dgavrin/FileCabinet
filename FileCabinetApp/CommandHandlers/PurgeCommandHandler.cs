using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// CommandHadler.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const string Command = "purge";

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
                Purge(appCommandRequest.Parameters);
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

        private static void Purge(string parameters)
        {
            if (Program.FileCabinetService is FileCabinetFileSystemService fileCabinetFileSystemService)
            {
                fileCabinetFileSystemService.Purge();
            }
        }
    }
}
