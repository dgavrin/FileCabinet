using System;
using System.Collections.Generic;

namespace FileCabinetGenerator
{
    /// <summary>
    /// The main program class of <see cref="global::FileCabinetGenerator"/>.
    /// </summary>
    public static class FileCabinetGenerator
    {
        private const string HintMessage = "Enter valid parameters: output-type, output file name, records-amount and start-id.";

        private static readonly Dictionary<string, Action<string>> CommandLineParameters = new Dictionary<string, Action<string>>
        {
            ["--OUTPUT-TYPE"] = new Action<string>(SetOutputType),
            ["-T"] = new Action<string>(SetOutputType),
            ["--OUTPUT"] = new Action<string>(SetOutput),
            ["-O"] = new Action<string>(SetOutput),
            ["--RECORDS-AMOUNT"] = new Action<string>(SetRecordsAmount),
            ["-A"] = new Action<string>(SetRecordsAmount),
            ["--START-ID"] = new Action<string>(SetStartId),
            ["-I"] = new Action<string>(SetStartId),
        };

        private static string outputType = "invalid";
        private static string fileName = "invalid";
        private static int recordsAmount = -3;
        private static int startId = -3;
        private static RecordsGenerator recordsGenerator;

        /// <summary>
        /// The main method of <see cref="global::FileCabinetGenerator"/>.
        /// </summary>
        public static void Main()
        {
            var commandLineArguments = Environment.GetCommandLineArgs()[1..];

            if (GetApplicationPreferences(commandLineArguments))
            {
                DisplayPreferences();

                FileCabinetGenerator.recordsGenerator = new RecordsGenerator(FileCabinetGenerator.startId, FileCabinetGenerator.recordsAmount);
                FileCabinetGenerator.recordsGenerator.GenerateRecords();
            }
            else
            {
                Console.WriteLine(HintMessage);
            }

            Console.WriteLine("Exiting an application...");
            Console.ReadKey();
        }

        private static bool GetApplicationPreferences(string[] commnandLineArguments)
        {
            if (commnandLineArguments == null || commnandLineArguments.Length < 4)
            {
                return false;
            }

            var temporaryArgument = new string[2];

            for (int i = 1; i <= commnandLineArguments.Length;)
            {
                temporaryArgument[0] = commnandLineArguments[i - 1];
                if (i != commnandLineArguments.Length)
                {
                    temporaryArgument[1] = commnandLineArguments[i];
                }

                var parsedCommand = CommandLineParameterParser(temporaryArgument);
                switch (parsedCommand)
                {
                    case TypeOfParameter.NotParameter:
                    case TypeOfParameter.FullParameter:
                        i++;
                        break;
                    case TypeOfParameter.ShortParameter:
                        i += 2;
                        break;
                }
            }

            return CheckPreferences();

            bool CheckPreferences()
            {
                var isValidPreferences = true;

                // Check output type.
                if (FileCabinetGenerator.outputType.Equals("invalid", StringComparison.InvariantCultureIgnoreCase))
                {
                    isValidPreferences = false;
                    Console.WriteLine("Invalid output type. Should be csv or xml.");
                }

                // Check output file name.
                if (FileCabinetGenerator.fileName.Equals("invalid", StringComparison.InvariantCultureIgnoreCase))
                {
                    isValidPreferences = false;
                }
                else if (FileCabinetGenerator.fileName.Equals("invalid output type", StringComparison.InvariantCultureIgnoreCase))
                {
                    isValidPreferences = false;
                }
                else if (FileCabinetGenerator.fileName.Equals("invalid file extension", StringComparison.InvariantCultureIgnoreCase))
                {
                    isValidPreferences = false;
                    Console.WriteLine("Invalid output file name. The file extension must match the type of output.");
                }

                // Check records amount.
                switch (FileCabinetGenerator.recordsAmount)
                {
                    case -1:
                        isValidPreferences = false;
                        Console.WriteLine("Invalid records amount. Should be more than 0.");
                        break;
                    case -2:
                        isValidPreferences = false;
                        Console.WriteLine("Invalid records amount. Should be integer.");
                        break;
                    case -3:
                        isValidPreferences = false;
                        Console.WriteLine("Records amount must be set.");
                        break;
                }

                // Check start id.
                switch (FileCabinetGenerator.startId)
                {
                    case -1:
                        isValidPreferences = false;
                        Console.WriteLine("Invalid start id. Should be more than 0.");
                        break;
                    case -2:
                        isValidPreferences = false;
                        Console.WriteLine("Invalid start id. Should be integer.");
                        break;
                    case -3:
                        isValidPreferences = false;
                        Console.WriteLine("Start id must be set.");
                        break;
                }

                return isValidPreferences;
            }
        }

        private static TypeOfParameter CommandLineParameterParser(string[] temporaryArgument)
        {
            var operation = string.Empty;
            var parameter = string.Empty;
            Action<string> changeApplicaionSetting = null;
            TypeOfParameter parsedCommand = TypeOfParameter.NotParameter;

            if (temporaryArgument[0].StartsWith("--", StringComparison.InvariantCulture))
            {
                var equalSignIndex = temporaryArgument[0].IndexOf("=", StringComparison.InvariantCulture);
                if (equalSignIndex > 0)
                {
                    operation = temporaryArgument[0].Substring(0, equalSignIndex);
                    parameter = temporaryArgument[0].Substring(equalSignIndex + 1);

                    if (CommandLineParameters.ContainsKey(operation.ToUpperInvariant()))
                    {
                        changeApplicaionSetting = CommandLineParameters[operation.ToUpperInvariant()];
                        parsedCommand = TypeOfParameter.FullParameter;
                    }
                }
            }
            else if (temporaryArgument[0].StartsWith("-", StringComparison.InvariantCulture))
            {
                operation = temporaryArgument[0];

                if (temporaryArgument[1] != null)
                {
                    parameter = temporaryArgument[1];
                }

                if (CommandLineParameters.ContainsKey(operation.ToUpperInvariant()))
                {
                    changeApplicaionSetting = CommandLineParameters[operation.ToUpperInvariant()];
                    parsedCommand = TypeOfParameter.ShortParameter;
                }
            }

            if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(parameter) && changeApplicaionSetting != null)
            {
#pragma warning disable CA1308
                changeApplicaionSetting.Invoke(parameter.ToLowerInvariant());
#pragma warning restore CA1308
            }

            return parsedCommand;
        }

        private static void SetOutputType(string outputType)
        {
            if (string.IsNullOrEmpty(outputType))
            {
                throw new ArgumentNullException(nameof(outputType));
            }

            if (outputType.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
            {
                FileCabinetGenerator.outputType = "csv";
            }
            else if (outputType.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                FileCabinetGenerator.outputType = "xml";
            }
        }

        private static void SetOutput(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!FileCabinetGenerator.outputType.Equals("invalid", StringComparison.InvariantCultureIgnoreCase))
            {
                if (fileName.EndsWith("." + outputType, StringComparison.InvariantCulture))
                {
                    FileCabinetGenerator.fileName = fileName;
                }
                else
                {
                    FileCabinetGenerator.fileName = "invalid file extension";
                }
            }
            else
            {
                FileCabinetGenerator.fileName = "invalid output type";
            }
        }

        private static void SetRecordsAmount(string recordsAmount)
        {
            if (string.IsNullOrEmpty(recordsAmount))
            {
                throw new ArgumentNullException(nameof(recordsAmount));
            }

            if (int.TryParse(recordsAmount, out int parsedRecordsAmount))
            {
                if (parsedRecordsAmount > 0)
                {
                    FileCabinetGenerator.recordsAmount = parsedRecordsAmount;
                }
                else
                {
                    FileCabinetGenerator.recordsAmount = -1; // Invalid records amount. Should be more than 0.
                }
            }
            else
            {
                FileCabinetGenerator.recordsAmount = -2; // Invalid records amount. Should be integer.
            }
        }

        private static void SetStartId(string startId)
        {
            if (string.IsNullOrEmpty(startId))
            {
                throw new ArgumentNullException(nameof(startId));
            }

            if (int.TryParse(startId, out int parsedStartId))
            {
                if (parsedStartId > 0)
                {
                    FileCabinetGenerator.startId = parsedStartId;
                }
                else
                {
                    FileCabinetGenerator.startId = -1; // Invalid start id. Should be more than 0.
                }
            }
            else
            {
                FileCabinetGenerator.startId = -2; // Invalid start id. Should be integer.
            }
        }

        private static void DisplayPreferences()
        {
            Console.WriteLine("Select Preferences:");
            Console.WriteLine($"Output-type: {FileCabinetGenerator.outputType}");
            Console.WriteLine($"Output: {FileCabinetGenerator.fileName}");
            Console.WriteLine($"Records-amount: {FileCabinetGenerator.recordsAmount}");
            Console.WriteLine($"Start-id: {FileCabinetGenerator.startId}");
            Console.WriteLine();
        }
    }
}
