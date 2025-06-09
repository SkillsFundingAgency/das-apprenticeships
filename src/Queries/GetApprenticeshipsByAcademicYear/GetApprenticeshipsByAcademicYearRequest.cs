namespace SFA.DAS.Learning.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearRequest : PagedQuery, IQuery
{
    public long UkPrn { get; }
    public int AcademicYear { get; }
    
    public GetApprenticeshipsByAcademicYearRequest(long ukPrn, int academicYear, int page, int? pageSize)
    {
        UkPrn = ukPrn;
        AcademicYear = academicYear;
        Page = page;
        PageSize = pageSize;
    }
}