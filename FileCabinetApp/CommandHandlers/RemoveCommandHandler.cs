using System;
using System.Globalization;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Remove command handler.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase, ICommandHandler
    {
        private const string Command = "remove";

        private ICommandHandler nextHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
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
                this.Remove(appCommandRequest.Parameters);
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

        private void Remove(string parameters)
        {
            if (parameters.Length == 0)
            {
                Console.WriteLine("Please try again. Enter record ID. 'remove <ID>'.");
                Console.WriteLine();
                return;
            }

            var recordIdForRemove = Convert.ToInt32(parameters, CultureInfo.InvariantCulture);

            if (this.fileCabinetService.Remove(recordIdForRemove))
            {
                Console.WriteLine($"Record #{recordIdForRemove} is removed.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"Record #{recordIdForRemove} doesn't exists.");
                Console.WriteLine();
            }
        }
    }
}
