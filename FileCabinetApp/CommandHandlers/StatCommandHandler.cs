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
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Stat(appCommandRequest.Parameters);
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

        private void Stat(string parameters)
        {
            var recordsCount = this.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.active} active record(s).");

            if (this.fileCabinetService is FileCabinetFileSystemService)
            {
                Console.WriteLine($"{recordsCount.removed} removed record(s).");
            }

            Console.WriteLine();
        }
    }
}
