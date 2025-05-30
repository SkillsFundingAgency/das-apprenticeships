using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearQueryHandler(
    IApprenticeshipQueryRepository queryRepository)
    : IQueryHandler<GetApprenticeshipsByAcademicYearRequest, GetApprenticeshipsByAcademicYearResponse>
{
    public async Task<GetApprenticeshipsByAcademicYearResponse> Handle(GetApprenticeshipsByAcademicYearRequest query, CancellationToken cancellationToken = default)
    {
        var academicYearDates = AcademicYearParser.ParseFrom(query.AcademicYear);
        
        var response  = await queryRepository.GetByDates(
            query.UkPrn,
            academicYearDates, 
            query.Limit,
            query.Offset,
            cancellationToken);

        return new GetApprenticeshipsByAcademicYearResponse
        {
            Items = response.Data.Select(apprenticeship => new GetApprenticeshipsByDatesResponseItem
            {
                Uln = apprenticeship.Uln
            }),
            PageSize = query.Limit,
            Page = query.Page,
            TotalItems = response.TotalItems,
        };
    }
}