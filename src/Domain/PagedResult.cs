namespace SFA.DAS.Apprenticeships.InnerApi.Responses;

/// <summary>
/// Returns a paged response
/// </summary>
/// <typeparam name="T">Data</typeparam>
public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }
}