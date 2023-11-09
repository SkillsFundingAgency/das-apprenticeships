using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apprenticeships.Queries.Exceptions;

namespace SFA.DAS.Apprenticeships.Queries
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResult> Send<TQuery, TResult>(TQuery query) where TQuery : IQuery
        {
            var service = _serviceProvider.GetService<IQueryHandler<TQuery, TResult>>();

            if (service == null)
            {
                throw new QueryDispatcherException($"Unable to dispatch query '{query.GetType().Name}'. No matching handler found.");
            }

            try
            {
                return service.Handle(query);
            }
            catch (Exception e)
            {
                throw new QueryException($"Unable to execute query '{query.GetType().Name}'.", e);
            }
        }

        public Task Send<TQuery>(TQuery query) where TQuery : IQuery
        {
            var service = _serviceProvider.GetService<IQueryHandler<TQuery>>();

            if (service == null)
            {
                throw new QueryDispatcherException($"Unable to dispatch query '{query.GetType().Name}'. No matching handler found.");
            }

            try
            {
                return service.Handle(query);
            }
            catch (Exception e)
            {
                throw new QueryException($"Unable to execute query '{query.GetType().Name}'.", e);
            }
        }
    }
}
