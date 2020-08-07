using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// List command handler.
    /// </summary>
    public class ListCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const string Command = "list";

        private ICommandHandler nextHandler;
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService)
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
                this.List(appCommandRequest.Parameters);
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

        private void List(string parameters)
        {
            var listOfRecords = this.fileCabinetService.GetRecords();
            this.fileCabinetService.DisplayRecords(listOfRecords);
            Console.WriteLine();
        }
    }
}
