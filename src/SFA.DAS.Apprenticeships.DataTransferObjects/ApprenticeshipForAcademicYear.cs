using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.DataTransferObjects;

[ExcludeFromCodeCoverage]
public record ApprenticeshipForAcademicYear(Guid Key, string Uln, DateTime StartDate, string LearningStatus)
{
    public Guid Key { get; } = Key;
    public string Uln { get; } = Uln;
    public DateTime StartDate { get; } = StartDate;
    public string LearningStatus { get; } = LearningStatus;
}
