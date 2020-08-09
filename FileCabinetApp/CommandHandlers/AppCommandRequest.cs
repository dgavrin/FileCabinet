using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Application command request.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The command parameters.</param>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public string Command { get; }

        /// <summary>
        /// Gets the command parameters.
        /// </summary>
        /// <value>
        /// The command parameters.
        /// </value>
        public string Parameters { get; }
    }
}
