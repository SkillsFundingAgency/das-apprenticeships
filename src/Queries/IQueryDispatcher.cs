namespace SFA.DAS.Apprenticeships.Queries
{
    public interface IQueryDispatcher
    {
        Task<TResult> Send<TQuery, TResult>(TQuery query) where TQuery : IQuery;
    }
}
