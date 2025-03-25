using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByDates;

public class GetApprenticeshipsByAcademicYearQueryHandler(
    IApprenticeshipQueryRepository queryRepository)
    : IQueryHandler<GetApprenticeshipsByDatesRequest, GetApprenticeshipsByDatesResponse>
{
    public async Task<GetApprenticeshipsByDatesResponse> Handle(GetApprenticeshipsByDatesRequest query, CancellationToken cancellationToken = default)
    {
        var academicYearDates = new DateRange(query.Dates.Start, query.Dates.End);
        
        var response  = await queryRepository.GetByDates(
            query.UkPrn,
            academicYearDates, 
            query.Page, 
            query.PageSize,
            query.Limit,
            query.Offset,
            cancellationToken);

        return new GetApprenticeshipsByDatesResponse
        {
            Items = response.Data.Select(apprenticeship => new GetApprenticeshipsByAcademicYearResponseItem
            {
                Uln = apprenticeship.Uln
            }),
            PageSize = response.PageSize,
            Page = response.Page,
            TotalItems = response.TotalItems,
        };
    }
}