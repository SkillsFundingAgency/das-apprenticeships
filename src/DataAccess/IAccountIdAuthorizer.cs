using SFA.DAS.Learning.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Learning.DataAccess
{
    public interface IAccountIdAuthorizer
    {
        void AuthorizeAccountId(Apprenticeship apprenticeship);
        IQueryable<Apprenticeship> ApplyAuthorizationFilterOnQueries(IQueryable<Apprenticeship> apprenticeships);
    }
}