using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Create command handler.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase, ICommandHandler
    {
        private const string Command = "create";

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
                Create(appCommandRequest.Parameters);
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

        private static void Create(string parameters)
        {
            bool invalidValues = true;

            do
            {
                try
                {
                    var newRecord = Program.FileCabinetService.SetInformationToRecord();
                    var recordId = Program.FileCabinetService.CreateRecord(newRecord);
                    Console.WriteLine($"Record #{recordId} is created.");
                    Console.WriteLine();

                    invalidValues = false;
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine($"Please try again. {ex.Message}");
                    Console.WriteLine();
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Please try again. {ex.Message}");
                    Console.WriteLine();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please try again and enter valid data.");
                    Console.WriteLine();
                }
            }
            while (invalidValues);
        }
    }
}
