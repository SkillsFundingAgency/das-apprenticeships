namespace SFA.DAS.Apprenticeships.Types;

public class PaymentsFrozenEvent
{
    public Guid ApprenticeshipKey { get; set; }


    public static implicit operator PaymentsFrozenEvent(SFA.DAS.Learning.Types.PaymentsFrozenEvent source)
    {
        return new PaymentsFrozenEvent
        {
            ApprenticeshipKey = source.LearningKey
        };
    }
}