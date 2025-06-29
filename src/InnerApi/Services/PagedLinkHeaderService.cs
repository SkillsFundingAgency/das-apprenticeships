using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using SFA.DAS.Learning.Queries;

namespace SFA.DAS.Learning.InnerApi.Services;

/// <summary>
/// IPagedLinkHeaderService
/// </summary>
public interface IPagedLinkHeaderService
{
    /// <summary>
    /// Adds 'Links' to response headers. 
    /// </summary>
    /// <param name="response">PagedQueryResult of T</param>
    /// <param name="request">PagedQuery</param>
    /// <typeparam name="T">Return type</typeparam>
    KeyValuePair<string, StringValues> GetPageLinks<T>(PagedQuery request, PagedQueryResult<T> response);
}

/// <summary>
/// Adds 'Links' to response headers. 
/// </summary>
public class PagedLinkHeaderService : IPagedLinkHeaderService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpContextAccessor">IHttpContextAccessor</param>
    public PagedLinkHeaderService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Adds 'Links' to response headers. 
    /// </summary>
    /// <param name="response">PagedQueryResult of T</param>
    /// <param name="request">PagedQuery</param>
    /// <typeparam name="T">Return type</typeparam>
    public KeyValuePair<string, StringValues> GetPageLinks<T>(PagedQuery request, PagedQueryResult<T> response)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var httpRequest = httpContext.Request;

        var baseUrl = GetBaseUrlFrom(httpRequest);

        var links = new List<string>();

        if (NotFirstPage(response))
        {
            var previousPage = request.Page - 1;
            var prevLink = QueryHelpers.AddQueryString(baseUrl, "page", previousPage.ToString());
            prevLink = QueryHelpers.AddQueryString(prevLink, "pageSize", response.PageSize.ToString());
            links.Add($"{prevLink};rel=\"prev\"");
        }

        if (NotLastPage(request, response))
        {
            var nextPage = request.Page + 1;
            var nextLink = QueryHelpers.AddQueryString(baseUrl, "page", nextPage.ToString());
            nextLink = QueryHelpers.AddQueryString(nextLink, "pageSize", response.PageSize.ToString());

            links.Add($"{nextLink};rel=\"next\"");
        }

        return new KeyValuePair<string, StringValues>("links", string.Join(",", links));
    }

    private static string GetBaseUrlFrom(HttpRequest httpRequest)
    {
        var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.Path}";

        foreach (var value in httpRequest.Query)
        {
            if (!value.Key.Equals("page", StringComparison.OrdinalIgnoreCase) && !value.Key.Equals("pageSize", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl = QueryHelpers.AddQueryString(baseUrl, value.Key, value.Value.ToString());
            }
        }

        return baseUrl;
    }


    private static bool NotLastPage<T>(PagedQuery request, PagedQueryResult<T> response)
    {
        return request.Offset < response.TotalItems - response.PageSize;
    }

    private static bool NotFirstPage<T>(PagedQueryResult<T> response)
    {
        return response.Page > 1;
    }
}