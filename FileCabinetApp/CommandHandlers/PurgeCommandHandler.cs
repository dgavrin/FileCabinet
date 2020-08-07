using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Purge command handler.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const string Command = "purge";

        private ICommandHandler nextHandler;
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
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
                this.Purge(appCommandRequest.Parameters);
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

        private void Purge(string parameters)
        {
            if (this.fileCabinetService is FileCabinetFileSystemService fileCabinetFileSystemService)
            {
                fileCabinetFileSystemService.Purge();
            }
        }
    }
}
