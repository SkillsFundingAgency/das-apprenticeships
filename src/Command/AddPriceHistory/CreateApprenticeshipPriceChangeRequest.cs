namespace SFA.DAS.Apprenticeships.Command.AddPriceHistory;

public class CreateApprenticeshipPriceChangeRequest : ICommand
{
    public CreateApprenticeshipPriceChangeRequest(long? providerId, long? employerId, Guid apprenticeshipKey,
        string userId, decimal? trainingPrice, decimal? assessmentPrice, decimal totalPrice, string reason)
    {
        ProviderId = providerId;
        EmployerId = employerId;
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;
        TrainingPrice = trainingPrice;
        AssessmentPrice = assessmentPrice;
        TotalPrice = totalPrice;
        Reason = reason;
    }

    public long? ProviderId { get; set; }
    public long? EmployerId { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public string UserId { get; set; }
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Reason { get; set; }
}