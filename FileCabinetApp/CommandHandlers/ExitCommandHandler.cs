using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Exit command handler.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const string Command = "exit";

        private ICommandHandler nextHandler;
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public ExitCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Exit(appCommandRequest.Parameters);
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

        private void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.IsRunning = false;

            if (this.fileCabinetService is FileCabinetFileSystemService fileCabinetFileSystemService)
            {
                fileCabinetFileSystemService.Dispose();
            }
        }
    }
}
