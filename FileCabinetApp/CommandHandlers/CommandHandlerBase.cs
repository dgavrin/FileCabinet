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
        public virtual void Handle(AppCommandRequest appCommandRequest)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
        }
    }
}
