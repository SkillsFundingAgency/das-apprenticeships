using SFA.DAS.Apprenticeships.Domain;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByDates;

public class GetApprenticeshipsByDatesRequest : PagedQuery, IQuery
{
    public long UkPrn { get; }
    public DateRange Dates { get; }

    public GetApprenticeshipsByDatesRequest(long ukPrn, DateRange dates, int page, int? pageSize)
    {
        UkPrn = ukPrn;
        Dates = dates;
        Page = page;
        PageSize = pageSize;
    }
}