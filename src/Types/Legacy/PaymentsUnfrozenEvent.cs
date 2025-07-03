namespace SFA.DAS.Apprenticeships.Types;

public class PaymentsUnfrozenEvent
{
    public Guid ApprenticeshipKey { get; set; }

    public static implicit operator PaymentsUnfrozenEvent(SFA.DAS.Learning.Types.PaymentsUnfrozenEvent source)
    {
        return new PaymentsUnfrozenEvent
        {
            ApprenticeshipKey = source.LearningKey
        };
    }
}