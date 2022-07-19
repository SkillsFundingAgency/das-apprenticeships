namespace SFA.DAS.Apprenticeships.Command
{
    public interface ICommandDispatcher
    {
        Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand;
    }
}
