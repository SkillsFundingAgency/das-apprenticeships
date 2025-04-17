namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearResponse : PagedQueryResult<GetApprenticeshipsByDatesResponseItem>;

public record GetApprenticeshipsByDatesResponseItem
{
    public string Uln { get; init; }
}