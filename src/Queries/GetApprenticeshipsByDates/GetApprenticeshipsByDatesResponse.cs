namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByDates;

public class GetApprenticeshipsByDatesResponse : PagedQueryResult<GetApprenticeshipsByAcademicYearResponseItem>
{
}

public record GetApprenticeshipsByAcademicYearResponseItem
{
    public string Uln { get; init; }
}