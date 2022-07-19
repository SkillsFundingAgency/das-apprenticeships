namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public interface IApprenticeshipFactory
    {
        Apprenticeship.Apprenticeship CreateNew(long uln, string trainingCode);
    }
}
