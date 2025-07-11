using SFA.DAS.Learning.DataAccess.Entities.Learning;

namespace SFA.DAS.Learning.DataAccess
{
    public interface IAccountIdAuthorizer
    {
        void AuthorizeAccountId(Entities.Learning.Learning learning);
        IQueryable<Entities.Learning.Learning> ApplyAuthorizationFilterOnQueries(IQueryable<Entities.Learning.Learning> apprenticeships);
    }
}