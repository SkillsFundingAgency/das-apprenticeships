namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearResponse : PagedQueryResult<GetApprenticeshipsByAcademicYearResponseItem>
{
}

public record GetApprenticeshipsByAcademicYearResponseItem
{
    public string Uln { get; init; }
}