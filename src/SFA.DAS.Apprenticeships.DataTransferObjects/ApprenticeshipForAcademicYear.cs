using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.DataTransferObjects;

[ExcludeFromCodeCoverage]
public record ApprenticeshipForAcademicYear
{
    public ApprenticeshipForAcademicYear(Guid key, string uln, DateTime startDate, string learningStatus)
    {
        Key = key;
        Uln = uln;
        StartDate = startDate;
        LearningStatus = learningStatus;
    }
    
    public Guid Key { get; set; }
    public string Uln { get; set; }
    public DateTime StartDate { get; set; }
    public string LearningStatus { get; set; }
}
