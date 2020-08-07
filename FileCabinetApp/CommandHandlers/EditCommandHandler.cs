using System;
using System.Globalization;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Edit command handler.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase, ICommandHandler
    {
        private const string Command = "edit";

        private ICommandHandler nextHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Edit(appCommandRequest.Parameters);
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

        private void Edit(string parameters)
        {
            if (parameters.Length == 0)
            {
                Console.WriteLine("Please try again. Enter record ID. 'edit <ID>'.");
                Console.WriteLine();
                return;
            }

            var recordIdForEdit = Convert.ToInt32(parameters, CultureInfo.InvariantCulture);
            var listOfRecords = this.fileCabinetService.GetRecords();

            foreach (var record in listOfRecords)
            {
                if (record.Id == recordIdForEdit)
                {
                    try
                    {
                        var editRecord = this.fileCabinetService.SetInformationToRecord();
                        this.fileCabinetService.EditRecord(recordIdForEdit, editRecord);
                        Console.WriteLine($"Record #{recordIdForEdit} is updated.");
                        Console.WriteLine();
                        return;
                    }
                    catch (ArgumentNullException ex)
                    {
                        Console.WriteLine($"Please try again. {ex.Message}");
                        Console.WriteLine();
                        return;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Please try again. {ex.Message}");
                        Console.WriteLine();
                        return;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Please try again and enter valid data.");
                        Console.WriteLine();
                        return;
                    }
                }
            }

            Console.WriteLine($"#{recordIdForEdit} record is not found.");
            Console.WriteLine();
        }
    }
}
