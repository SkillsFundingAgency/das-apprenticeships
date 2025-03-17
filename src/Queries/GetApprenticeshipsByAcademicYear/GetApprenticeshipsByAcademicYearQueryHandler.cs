using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient.Calendar;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearQueryHandler : IQueryHandler<GetApprenticeshipsByAcademicYearRequest, GetApprenticeshipsByAcademicYearResponse>
{
    private readonly IApprenticeshipsOuterApiClient _apprenticeshipsOuterApiClient;
    private readonly IApprenticeshipQueryRepository _queryRepository;

    public GetApprenticeshipsByAcademicYearQueryHandler(IApprenticeshipQueryRepository queryRepository, IApprenticeshipsOuterApiClient apprenticeshipsOuterApiClient)
    {
        _queryRepository = queryRepository;
        _apprenticeshipsOuterApiClient = apprenticeshipsOuterApiClient;
    }
    
    public async Task<GetApprenticeshipsByAcademicYearResponse> Handle(GetApprenticeshipsByAcademicYearRequest query, CancellationToken cancellationToken = default)
    {
        //var academicYearsResponse = await _apprenticeshipsOuterApiClient.GetAcademicYear(query.AcademicYear);

        var academicYearsResponse = new GetAcademicYearsResponse
        {
            StartDate = new DateTime(2023, 9, 1),
            EndDate = new DateTime(2024, 8, 31)
        };

        var academicYearDates = new DateRange(academicYearsResponse.StartDate, academicYearsResponse.EndDate);
        
        var response  = await _queryRepository.GetAllForAcademicYear(
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
            }).ToList(),
            PageSize = response.PageSize,
            Page = response.Page,
            TotalItems = response.TotalItems,
        };
    }
}