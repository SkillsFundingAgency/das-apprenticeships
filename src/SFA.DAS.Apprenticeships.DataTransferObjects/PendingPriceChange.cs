﻿using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.DataTransferObjects;

[ExcludeFromCodeCoverage]
public class PendingPriceChange
{
	public decimal? OriginalTrainingPrice { get; set; }
	public decimal? OriginalAssessmentPrice { get; set; }
	public decimal OriginalTotalPrice { get; set; }
	public decimal? PendingTrainingPrice { get; set; }
    public decimal? PendingAssessmentPrice { get; set; }
    public decimal PendingTotalPrice { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public string? Reason { get; set; }
    public long? Ukprn { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string Requester { get; set; }
}