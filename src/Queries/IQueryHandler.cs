namespace SFA.DAS.Learning.Queries
{
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery
    {
        Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
    }

    public interface IQueryHandler<in TQuery> where TQuery : IQuery
    {
        Task Handle(TQuery query, CancellationToken cancellationToken = default);
    }
}
