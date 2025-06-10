namespace SFA.DAS.Learning.Domain;

/// <summary>
/// Returns a paged response
/// </summary>
/// <typeparam name="T">Data</typeparam>
public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}