﻿using SFA.DAS.Apprenticeships.DataAccess;

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
