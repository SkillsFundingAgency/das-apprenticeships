﻿using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.DataTransferObjects;

[ExcludeFromCodeCoverage]
public class PendingPriceChange
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
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
    public string? Initiator { get; set; }
    public DateTime? ProviderApprovedDate { get; set; }
    public DateTime? EmployerApprovedDate { get; set; }
}