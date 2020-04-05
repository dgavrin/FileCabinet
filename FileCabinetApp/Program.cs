using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Denis Gavrin";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;
        private static FileCabinetService fileCabinetService = new FileCabinetService();
        private static CultureInfo cultureEnUS = new CultureInfo("en-US");

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the statistics by records", "The 'stat' command prints the statistics by records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "returns a list of records added to the service", "The 'list' command returns a list of records added to the service." },
            new string[] { "edit", "edits a record", "The 'edit' command edits a record." },
            new string[] { "find", "finds records for the specified key", "The 'find' command finds records for the specified key" },
        };

        private static Tuple<string, Func<string, FileCabinetRecord[]>>[] searchCommands = new Tuple<string, Func<string, FileCabinetRecord[]>>[]
        {
            new Tuple<string, Func<string, FileCabinetRecord[]>>("firstname", Program.fileCabinetService.FindByFirstName),
            new Tuple<string, Func<string, FileCabinetRecord[]>>("lastname", Program.fileCabinetService.FindByLastName),
            new Tuple<string, Func<string, FileCabinetRecord[]>>("dateofbirth", Program.fileCabinetService.FindByDateOfBirth),
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
            Console.WriteLine();
        }

        private static void Create(string parameters)
        {
            bool invalidValues = true;

            do
            {
                try
                {
                    Console.Write("First Name: ");
                    var firstName = Console.ReadLine();

                    Console.Write("Last Name: ");
                    var lastName = Console.ReadLine();

                    Console.Write("Date of birth (MM/DD/YYYY): ");
                    var dateOfBirth = DateTime.Parse(Console.ReadLine(), Program.cultureEnUS);

                    Console.WriteLine("Wallet (from 0): ");
                    var wallet = decimal.Parse(Console.ReadLine(), Program.cultureEnUS);

                    Console.WriteLine("Marital status ('M' - married, 'U' - unmarried): ");
                    var maritalStatus = char.MinValue;
                    var married = Console.ReadLine();
                    if (married.Length > 0)
                    {
                        maritalStatus = married[0];
                    }

                    Console.WriteLine("Height (more than 0): ");
                    var height = short.Parse(Console.ReadLine(), Program.cultureEnUS);

                    var recordId = Program.fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, wallet, maritalStatus, height);
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

        private static void List(string parameters)
        {
            var listOfRecords = Program.fileCabinetService.GetRecords();

            FileCabinetService.DisplayRecords(listOfRecords);
            Console.WriteLine();
        }

        private static void Edit(string parameters)
        {
            if (parameters.Length == 0)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var recordIdForEdit = Convert.ToInt32(parameters, CultureInfo.InvariantCulture);
            var listOfRecords = Program.fileCabinetService.GetRecords();

            foreach (var record in listOfRecords)
            {
                if (record.Id == recordIdForEdit)
                {
                    try
                    {
                        Console.Write("First Name: ");
                        var firstName = Console.ReadLine();

                        Console.Write("Last Name: ");
                        var lastName = Console.ReadLine();

                        Console.Write("Date of birth (MM/DD/YYYY): ");
                        var dateOfBirth = DateTime.Parse(Console.ReadLine(), Program.cultureEnUS);

                        Console.WriteLine("Wallet (from 0): ");
                        var wallet = decimal.Parse(Console.ReadLine(), Program.cultureEnUS);

                        Console.WriteLine("Marital status ('M' - married, 'U' - unmarried): ");
                        char maritalStatus = char.MinValue;
                        var married = Console.ReadLine();
                        if (married.Length > 0)
                        {
                            maritalStatus = married[0];
                        }

                        Console.WriteLine("Height (more than 0): ");
                        var height = short.Parse(Console.ReadLine(), Program.cultureEnUS);

                        Program.fileCabinetService.EditRecord(recordIdForEdit, firstName, lastName, dateOfBirth, wallet, maritalStatus, height);
                        Console.WriteLine($"Record #{recordIdForEdit} is updated.");
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
        }

        private static void Find(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                string[] inputParameters = parameters.Split(' ', 2);

                if (inputParameters.Length < 2)
                {
                    Console.WriteLine("Please try again. Enter the key. The syntax for the 'find' command is \"find <search by> <key> \".");
                    Console.WriteLine();
                    return;
                }

                const int commandIndex = 0;
                const int argumentIndex = 1;
                var command = inputParameters[commandIndex];
                var argument = inputParameters[argumentIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine($"Please try again. The '{command}' is invalid parameter.");
                    Console.WriteLine();
                    return;
                }

                var index = Array.FindIndex(searchCommands, 0, searchCommands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    var foundRecords = searchCommands[index].Item2(argument);

                    if (foundRecords.Length == 0)
                    {
                        Console.WriteLine($"There are no entries with parameter '{argument}'.");
                    }
                    else
                    {
                        FileCabinetService.DisplayRecords(foundRecords);
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine($"Search by {command} is not possible.");
                }
            }
            else
            {
                Console.WriteLine("Error entering parameters. The syntax for the 'find' command is \"find <search by> <key> \".");
                Console.WriteLine();
            }
        }
    }
}