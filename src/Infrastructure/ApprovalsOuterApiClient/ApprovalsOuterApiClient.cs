using System.Net;
using Newtonsoft.Json;

namespace SFA.DAS.Apprenticeships.Infrastructure.ApprovalsOuterApiClient;

public class ApprovalsOuterApiClient : IApprovalsOuterApiClient
{
    private readonly HttpClient _httpClient;

    private const string GetStandardUrl = "TrainingCourses/standards";

    public ApprovalsOuterApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> GetFundingBandMaximum(int courseCode)
    {
        var response = await _httpClient.GetAsync(GetStandardUrl).ConfigureAwait(false);

        if (response.StatusCode.Equals(HttpStatusCode.NotFound))
        {
            throw new Exception("Standard not found.");
        }

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var standardResponse = JsonConvert.DeserializeObject<StandardResponse>(json);
            var summaries = FilterResponse(standardResponse);
            return summaries.Single(x => x.LarsCode == courseCode).FundingBandMaximum;
        }

        throw new Exception($"Status code: {response.StatusCode} returned from approvals outer api.");
    }

    private IEnumerable<StandardSummary> FilterResponse(StandardResponse response)
    {
        var statusList = new string[] { "Approved for delivery", "Retired" };
        var filteredStandards = response.Standards.Where(s => statusList.Contains(s.Status));

        var latestVersionsOfStandards = filteredStandards.
            GroupBy(s => s.LarsCode).
            Select(c => c.OrderByDescending(x => x.VersionMajor).ThenByDescending(y => y.VersionMinor).FirstOrDefault());

        var latestVersionsStandardUIds = latestVersionsOfStandards.Select(s => s.StandardUId);

        foreach (var latestStandard in filteredStandards.Where(s => latestVersionsStandardUIds.Contains(s.StandardUId)))
        {
            latestStandard.IsLatestVersion = true;
        }

        return filteredStandards;
    }
}