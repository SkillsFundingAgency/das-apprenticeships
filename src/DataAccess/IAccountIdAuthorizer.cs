using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.DataAccess
{
    public interface IAccountIdAuthorizer
    {
        void AuthorizeAccountId(Apprenticeship apprenticeship);
        IQueryable<Apprenticeship> ApplyAuthorizationFilterOnQueries(DbSet<Apprenticeship> apprenticeships);
    }
}