namespace SFA.DAS.Apprenticeships.Domain.Repositories
{
    public interface IApprenticeshipRepository
    {
        Task Add(Apprenticeship.Apprenticeship apprenticeship);
    }
}
