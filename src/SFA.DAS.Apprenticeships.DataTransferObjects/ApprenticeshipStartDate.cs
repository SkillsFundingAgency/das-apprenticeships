﻿namespace SFA.DAS.Apprenticeships.DataTransferObjects;

public class ApprenticeshipStartDate
{
    public Guid ApprenticeshipKey { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public long UKPRN { get; set; }
    public DateTime ApprenticeDateOfBirth { get; set; }
    public string CourseCode { get; set; }
    public string? CourseVersion { get; set; }
    public DateTime SimplifiedPaymentsMinimumStartDate { get; set; }
}
