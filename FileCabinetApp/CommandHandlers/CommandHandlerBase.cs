using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// CommandHandlerBase.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <inheritdoc/>
        public abstract void Handle(AppCommandRequest appCommandRequest);

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
        }
    }
}
