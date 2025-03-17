namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

public class GetApprenticeshipsByAcademicYearRequest : PagedQuery, IQuery
{
    public long UkPrn { get; }
    public string AcademicYear { get; }
    public int Page { get; }
    public int? PageSize { get; }

    public GetApprenticeshipsByAcademicYearRequest(long ukPrn, string academicYear, int page, int? pageSize)
    {
        UkPrn = ukPrn;
        AcademicYear = academicYear;
        Page = page;
        PageSize = pageSize;
    }
}