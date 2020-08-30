using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Delete command handler.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase, ICommandHandler
    {
        private const string Command = "delete";

        private ICommandHandler nextHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Delete(appCommandRequest.Parameters);
                Memoizer.GetMemoizer(this.fileCabinetService).Clear();
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

        private void Delete(string parameters)
        {
            const int keyIndex = 1;
            const int valueIndex = 2;
            const string invalidCommandSyntaxMessage = "Incorrect syntax for 'delete' command.";

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters), "The list of parameters for the 'delete' command cannot be empty.");
                }

                var parametersRegex = new Regex(@"where (.*)=(.*)", RegexOptions.IgnoreCase);
                if (parametersRegex.IsMatch(parameters))
                {
                    var matchParameters = parametersRegex.Match(parameters);
                    var key = matchParameters.Groups[keyIndex].Value.ToUpperInvariant().Trim(' ');
                    var value = Regex.Match(matchParameters.Groups[valueIndex].Value, @"'(.*?)'").Groups[1].Value.Trim(' ');

                    if (!(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)))
                    {
                        var identifiers = this.fileCabinetService.Delete(key, value);
                        var stringOfIdentifiers = new StringBuilder();

                        for (int i = 0; i < identifiers.Count; i++)
                        {
                            stringOfIdentifiers.Append($"#{identifiers[i]}");
                            if (i < identifiers.Count - 1)
                            {
                                stringOfIdentifiers.Append(", ");
                            }
                        }

                        Console.Write($"Record {stringOfIdentifiers}");
                        if (identifiers.Count == 1)
                        {
                            Console.WriteLine(" is deleted.");
                        }
                        else
                        {
                            Console.WriteLine(" are deleted.");
                        }

                        Console.WriteLine();
                    }
                    else
                    {
                        throw new ArgumentException(invalidCommandSyntaxMessage);
                    }
                }
                else
                {
                    throw new ArgumentException(invalidCommandSyntaxMessage);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"The records have not been deleted. {ex.Message}");
                Console.WriteLine("Example 'delete' command: delete where id = '1'.");
                Console.WriteLine();
            }
        }
    }
}
