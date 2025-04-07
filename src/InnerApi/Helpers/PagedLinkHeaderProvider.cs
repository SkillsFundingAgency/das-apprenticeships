using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using SFA.DAS.Apprenticeships.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.Helpers;

public interface IPagedLinkHeaderProvider
{
    void AddPageLinks<T>(PagedQueryResult<T> response, PagedQuery query);
}

public class PagedLinkHeaderProvider : IPagedLinkHeaderProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PagedLinkHeaderProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void AddPageLinks<T>(PagedQueryResult<T> response, PagedQuery query)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var httpRequest = httpContext.Request;

        var baseUrl = GetBaseUrlFrom(httpRequest);

        var links = new List<string>();
        
        if (response.Page > 1)
        {
            var previousPage = query.Page - 1;
            var prevLink = QueryHelpers.AddQueryString(baseUrl, "page", previousPage.ToString());
            prevLink = QueryHelpers.AddQueryString(prevLink, "pageSize", response.PageSize.ToString());
            links.Add($"{prevLink};rel=\"prev\"");
        }


        // if (ShouldDisplayPrevAndFirstLink(query.Offset))
        // {
        //     var firstLink = QueryHelpers.AddQueryString(baseUrl, "page", 1.ToString());
        //     firstLink = QueryHelpers.AddQueryString(firstLink, "pageSize", response.PageSize.ToString());
        //
        //    
        //     links.Add($"{firstLink};rel=\"first\"");
        // }
        
        // if (response.Page < response.TotalPages)
        // {
        //     var lastLink = QueryHelpers.AddQueryString(baseUrl, "page", response.TotalPages.ToString());
        //     lastLink = QueryHelpers.AddQueryString(lastLink, "pageSize", response.PageSize.ToString());
        //     links.Add($"{lastLink};rel=\"last\"");
        // }

        if (ShouldDisplayNext(query.Offset, response.TotalItems, response.PageSize))
        {
            var nextPage = query.Page + 1;
            var nextLink = QueryHelpers.AddQueryString(baseUrl, "page", nextPage.ToString());
            nextLink = QueryHelpers.AddQueryString(nextLink, "pageSize", response.PageSize.ToString());
        
            links.Add($"{nextLink};rel=\"next\"");
        }

        _httpContextAccessor.HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("links", string.Join(",", links)));
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

     private static bool ShouldDisplayNext(int offset, int totalItems, int pageSize) => offset < totalItems - pageSize;
    //
    // private static bool ShouldDisplayPrevAndFirstLink(int offset) => offset > 0;
}