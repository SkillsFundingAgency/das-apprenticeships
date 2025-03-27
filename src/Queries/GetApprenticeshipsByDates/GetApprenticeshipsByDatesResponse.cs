namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByDates;

public class GetApprenticeshipsByDatesResponse : PagedQueryResult<GetApprenticeshipsByDatesResponseItem>
{
}

public record GetApprenticeshipsByDatesResponseItem
{
    public string Uln { get; init; }
}