namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearRequest : PagedQuery, IQuery
{
    public long UkPrn { get; }
    public DateTime AcademicYear { get; }
    
    public GetApprenticeshipsByAcademicYearRequest(long ukPrn, DateTime academicYear, int page, int? pageSize)
    {
        UkPrn = ukPrn;
        AcademicYear = academicYear;
        Page = page;
        PageSize = pageSize;
    }
}