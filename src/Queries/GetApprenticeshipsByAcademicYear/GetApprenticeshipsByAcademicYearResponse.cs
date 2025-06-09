namespace SFA.DAS.Learning.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearResponse : PagedQueryResult<GetApprenticeshipsByDatesResponseItem>;

public record GetApprenticeshipsByDatesResponseItem
{
    public string Uln { get; init; }
}