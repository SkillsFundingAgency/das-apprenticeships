namespace SFA.DAS.Apprenticeships.Command
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task Handle(T command, CancellationToken cancellationToken = default(CancellationToken));
    }
}
