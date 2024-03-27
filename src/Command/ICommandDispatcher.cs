namespace SFA.DAS.Apprenticeships.Command
{
    public interface ICommandDispatcher
    {
        Task<TReturnType> Send<TCommand, TReturnType>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand;
        Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand;
    }
}
