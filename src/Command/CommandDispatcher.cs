using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Apprenticeships.Command
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

            if (handler == null)
            {
                throw new CommandDispatcherException($"Unable to dispatch command '{command.GetType().Name}'. No matching handler found.");
            }

            return handler.Handle(command, cancellationToken);
        }

        public Task<TReturnType> Send<TCommand, TReturnType>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TReturnType>>();

            if (handler == null)
            {
                throw new CommandDispatcherException($"Unable to dispatch command '{command.GetType().Name}'. No matching handler found.");
            }

            return handler.Handle(command, cancellationToken);
        }
    }
}