using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.DataAccess
{
    public interface IAccountIdAuthorizer
    {
        void ValidateAccountIds(Apprenticeship apprenticeship);
        void AuthorizeApprenticeshipQueries(ModelBuilder modelBuilder);
    }
}