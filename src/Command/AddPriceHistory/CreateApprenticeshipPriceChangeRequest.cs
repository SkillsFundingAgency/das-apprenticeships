namespace SFA.DAS.Apprenticeships.Command.AddPriceHistory;

public class CreateApprenticeshipPriceChangeRequest : ICommand
{
    public CreateApprenticeshipPriceChangeRequest(string requester, Guid apprenticeshipKey,
        string userId, decimal? trainingPrice, decimal? assessmentPrice, decimal totalPrice, string reason,
        DateTime effectiveFromDate)
    {
        Requester = requester;
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;
        TrainingPrice = trainingPrice;
        AssessmentPrice = assessmentPrice;
        TotalPrice = totalPrice;
        Reason = reason;
        EffectiveFromDate = effectiveFromDate;
    }

    public string Requester { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public string UserId { get; set; }
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Reason { get; set; }
    public DateTime EffectiveFromDate { get; set; }
}