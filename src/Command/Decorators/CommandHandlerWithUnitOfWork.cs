using SFA.DAS.Learning.DataAccess;

namespace SFA.DAS.Learning.Command.Decorators
{
    public class CommandHandlerWithUnitOfWork<T> : ICommandHandler<T> where T : ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly LearningDataContext _dataContext;

        public CommandHandlerWithUnitOfWork(
            ICommandHandler<T> handler,
            LearningDataContext dataContext)
        {
            _handler = handler;
            _dataContext = dataContext;
        }

        public async Task Handle(T command, CancellationToken cancellationToken = default)
        {
            var transaction = await _dataContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _handler.Handle(command, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
