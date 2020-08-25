using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Update command handler.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase, ICommandHandler
    {
        private const string Command = "update";

        private ICommandHandler nextHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Update(appCommandRequest.Parameters);
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

        private static List<KeyValuePair<string, string>> GetKeyValuePairsOfParameters(string parameters)
        {
            const int keyIndex = 1;
            const int valueIndex = 2;

            if (string.IsNullOrEmpty(parameters))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var keyValuePairs = new List<KeyValuePair<string, string>>();
            var separatedParameters = parameters.Split(", ");
            var parameterRegex = new Regex(@"(.*)=(.*)");

            foreach (var parameter in separatedParameters)
            {
                if (parameterRegex.IsMatch(parameter))
                {
                    var match = parameterRegex.Match(parameter);
                    var key = match.Groups[keyIndex].Value.Trim(' ').ToUpperInvariant();
                    var value = Regex.Match(match.Groups[valueIndex].Value, @"'(.*?)'").Groups[1].Value.Trim(' ');
                    keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
                }
                else
                {
                    throw new ArgumentException("One of the entered parameters is incorrect.");
                }
            }

            return keyValuePairs;
        }

        private static List<KeyValuePair<string, string>> GetKeyValuePairsOfSearchOptions(string searchOptions)
        {
            const int keyIndex = 1;
            const int valueIndex = 2;

            if (string.IsNullOrEmpty(searchOptions))
            {
                throw new ArgumentNullException(nameof(searchOptions));
            }

            var keyValuePairs = new List<KeyValuePair<string, string>>();
            var separatedSearchOptions = searchOptions.Split(" and ");
            var optionRegex = new Regex(@"(.*)=(.*)");

            foreach (var option in separatedSearchOptions)
            {
                if (optionRegex.IsMatch(option))
                {
                    var match = optionRegex.Match(option);
                    var key = match.Groups[keyIndex].Value.Trim(' ').ToUpperInvariant();
                    var value = Regex.Match(match.Groups[valueIndex].Value, @"'(.*?)'").Groups[1].Value.Trim(' ');
                    keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
                }
                else
                {
                    throw new ArgumentException("One of the entered search parameters is incorrect.");
                }
            }

            return keyValuePairs;
        }

        private void Update(string parameters)
        {
            const int newRecordParametersIndex = 1;
            const int searchOptionsIndex = 2;
            const string invalidCommandSyntaxMessage = "Incorrect syntax for 'delete' command.";

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters), "The list of parameters for the 'update' command cannot be empty.");
                }

                var parametersRegex = new Regex(@"set (.*) where (.*)", RegexOptions.IgnoreCase);
                if (parametersRegex.IsMatch(parameters))
                {
                    var matchParameters = parametersRegex.Match(parameters);
                    var newRecordParameters = GetKeyValuePairsOfParameters(matchParameters.Groups[newRecordParametersIndex].Value);
                    var recordSearchOptions = GetKeyValuePairsOfSearchOptions(matchParameters.Groups[searchOptionsIndex].Value);

                    var identifiers = this.fileCabinetService.Update(newRecordParameters, recordSearchOptions);
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
                        Console.WriteLine(" is updated.");
                    }
                    else
                    {
                        Console.WriteLine(" are updated.");
                    }

                    Console.WriteLine();
                }
                else
                {
                    throw new ArgumentException(invalidCommandSyntaxMessage);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"The records have not been updated. {ex.Message}");
                Console.WriteLine("Examples 'update' command:");
                Console.WriteLine("update set firstname = 'Salvador', lastname = 'Harris' where id = '1';");
                Console.WriteLine("update set firstname = 'Salvador', lastname = 'Harris' where id = '1' and dateofbirth = '11.11.1999'.");
                Console.WriteLine();
            }
        }
    }
}
