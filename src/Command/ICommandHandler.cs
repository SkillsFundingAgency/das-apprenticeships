using SFA.DAS.Apprenticeships.Domain;

namespace SFA.DAS.Apprenticeships.Command
{
    public interface ICommandHandler<in T, TResult> where T : ICommand
    {
        Task<TResult> Handle(T command, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface ICommandHandler<in T> where T : ICommand
    {
        Task Handle(T command, CancellationToken cancellationToken = default(CancellationToken));
    }
}
