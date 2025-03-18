using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearQueryHandler(
    IApprenticeshipQueryRepository queryRepository,
    IApprenticeshipsOuterApiClient apprenticeshipsOuterApiClient)
    : IQueryHandler<GetApprenticeshipsByAcademicYearRequest, GetApprenticeshipsByAcademicYearResponse>
{
    public async Task<GetApprenticeshipsByAcademicYearResponse> Handle(GetApprenticeshipsByAcademicYearRequest query, CancellationToken cancellationToken = default)
    {
        var academicYearsResponse = await apprenticeshipsOuterApiClient.GetAcademicYear(query.AcademicYear);

        var academicYearDates = new DateRange(academicYearsResponse.StartDate, academicYearsResponse.EndDate);
        
        var response  = await queryRepository.GetForAcademicYear(
            query.UkPrn,
            academicYearDates, 
            query.Page, 
            query.PageSize,
            query.Limit,
            query.Offset,
            cancellationToken);

        return new GetApprenticeshipsByAcademicYearResponse
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