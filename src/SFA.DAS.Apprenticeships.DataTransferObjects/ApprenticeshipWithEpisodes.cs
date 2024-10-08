using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.DataTransferObjects;

[ExcludeFromCodeCoverage]
public class ApprenticeshipWithEpisodes
{
    public ApprenticeshipWithEpisodes(Guid key, string uln, DateTime startDate, DateTime plannedEndDate, List<Episode> episodes, int ageAtStartOfApprenticeship)
    {
        Key = key;
        Uln = uln;
        StartDate = startDate;
        PlannedEndDate = plannedEndDate;
        Episodes = episodes;
        AgeAtStartOfApprenticeship = ageAtStartOfApprenticeship;
    }

    public Guid Key { get; set; }
    public string Uln { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public List<Episode> Episodes { get; set; }
    public int AgeAtStartOfApprenticeship { get; set; }
}