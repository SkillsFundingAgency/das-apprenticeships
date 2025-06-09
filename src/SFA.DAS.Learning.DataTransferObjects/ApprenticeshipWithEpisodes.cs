using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Learning.DataTransferObjects;

[ExcludeFromCodeCoverage]
public class ApprenticeshipWithEpisodes
{
    public ApprenticeshipWithEpisodes(
        Guid key, string uln, DateTime startDate, DateTime plannedEndDate, List<Episode> episodes, int ageAtStartOfApprenticeship, DateTime? withdrawnDate)
    {
        Key = key;
        Uln = uln;
        StartDate = startDate;
        PlannedEndDate = plannedEndDate;
        Episodes = episodes;
        AgeAtStartOfApprenticeship = ageAtStartOfApprenticeship;
        WithdrawnDate = withdrawnDate;
    }

    public Guid Key { get; set; }
    public string Uln { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public List<Episode> Episodes { get; set; }
    public int AgeAtStartOfApprenticeship { get; set; }
    public DateTime? WithdrawnDate { get; set; }
}