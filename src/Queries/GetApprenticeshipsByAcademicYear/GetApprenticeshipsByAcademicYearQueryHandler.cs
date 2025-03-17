using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearQueryHandler : IQueryHandler<GetApprenticeshipsByAcademicYearRequest, GetApprenticeshipsByAcademicYearResponse>
{
    private readonly AcademicYearParser _academicYearParser;
    private readonly IApprenticeshipQueryRepository _queryRepository;

    public GetApprenticeshipsByAcademicYearQueryHandler(AcademicYearParser academicYearParser, IApprenticeshipQueryRepository queryRepository)
    {
        _academicYearParser = academicYearParser;
        _queryRepository = queryRepository;
    }
    
    public async Task<GetApprenticeshipsByAcademicYearResponse> Handle(GetApprenticeshipsByAcademicYearRequest query, CancellationToken cancellationToken = default)
    {
        var academicYearDates = _academicYearParser.ParseFrom(query.AcademicYear);
        
        var response  = await _queryRepository.GetAllForAcademicYear(
            query.UkPrn,
            academicYearDates, 
            query.Page, 
            query.PageSize,
            query.Limit,
            query.Offset);

        return new GetApprenticeshipsByAcademicYearResponse
        {
            Items = response.Data.ToList(),
            PageSize = response.PageSize,
            Page = response.Page,
            TotalItems = response.TotalItems,
        };
    }
}