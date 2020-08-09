using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// IcommandHandler.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets the next handler.
        /// </summary>
        /// <param name="commandHandler">The handler.</param>
        public void SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="appCommandRequest">The request.</param>
        public void Handle(AppCommandRequest appCommandRequest);
    }
}
