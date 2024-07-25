﻿namespace SFA.DAS.Apprenticeships.Types;

public class OldApprenticeshipStartDateChangedEvent
{
    public Guid ApprenticeshipKey { get; set; }
    public long ApprenticeshipId { get; set; }
    public DateTime ActualStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int? AgeAtStartOfApprenticeship { get; set; }
    public long EmployerAccountId { get; set; }
    public long ProviderId { get; set; }
    public DateTime ApprovedDate { get; set; }
    public string ProviderApprovedBy { get; set; }
    public string EmployerApprovedBy { get; set; }
    public string Initiator { get; set; }
    public Guid ApprenticeshipEpisodeKey { get; set; }
    public Guid PriceKey { get; set; }
    public List<Guid> DeletedPriceKeys { get; set; }
}