﻿namespace SFA.DAS.Apprenticeships.Command.CreatePriceChange;

public class CreatePriceChangeCommand : ICommand
{
    public CreatePriceChangeCommand(
		string initiator,
		Guid apprenticeshipKey,
        string userId, 
        decimal? trainingPrice, 
        decimal? assessmentPrice, 
        decimal totalPrice, 
        string reason,
        DateTime effectiveFromDate)
    {
        Initiator = initiator;
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;
        TrainingPrice = trainingPrice;
        AssessmentPrice = assessmentPrice;
        TotalPrice = totalPrice;
        Reason = reason;
        EffectiveFromDate = effectiveFromDate;
    }
    public string Initiator { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public string UserId { get; set; }
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Reason { get; set; }
    public DateTime EffectiveFromDate { get; set; }
}