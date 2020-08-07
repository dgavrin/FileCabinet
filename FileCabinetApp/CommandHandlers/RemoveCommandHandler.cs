using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Remove command handler.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const string Command = "remove";

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
                Remove(appCommandRequest.Parameters);
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

        private static void Remove(string parameters)
        {
            if (parameters.Length == 0)
            {
                Console.WriteLine("Please try again. Enter record ID. 'remove <ID>'.");
                Console.WriteLine();
                return;
            }

            var recordIdForRemove = Convert.ToInt32(parameters, CultureInfo.InvariantCulture);

            if (Program.FileCabinetService.Remove(recordIdForRemove))
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
