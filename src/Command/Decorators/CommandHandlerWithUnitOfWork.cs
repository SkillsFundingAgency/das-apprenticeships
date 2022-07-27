using SFA.DAS.Apprenticeships.DataAccess;

namespace SFA.DAS.Apprenticeships.Command.Decorators
{
    public class CommandHandlerWithUnitOfWork<T> : ICommandHandler<T> where T : ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly ApprenticeshipsDataContext _dataContext;

        public CommandHandlerWithUnitOfWork(
            ICommandHandler<T> handler,         
            ApprenticeshipsDataContext dataContext)
        {
            _handler = handler;
            _dataContext = dataContext;
        }

        public async Task Handle(T command, CancellationToken cancellationToken = default)
        {
            await _handler.Handle(command, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);
        }
    }
}
