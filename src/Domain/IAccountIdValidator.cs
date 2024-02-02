using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain
{
    public interface IAccountIdValidator
    {
        void ValidateAccountId(ApprenticeshipDomainModel apprenticeship);
    }
}
