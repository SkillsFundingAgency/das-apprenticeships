using System.Net;
using Newtonsoft.Json;

namespace SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

public class ApprenticeshipsOuterApiClient : IApprenticeshipsOuterApiClient
{
    private readonly HttpClient _httpClient;

    private const string GetStandardUrl = "TrainingCourses/standards";

    public ApprenticeshipsOuterApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<GetStandardResponse> GetStandard(int courseCode)
    {
        var response = await _httpClient.GetAsync(Path.Combine(GetStandardUrl, courseCode.ToString())).ConfigureAwait(false);

        if (response.StatusCode.Equals(HttpStatusCode.NotFound))
        {
            throw new Exception("Standard not found.");
        }

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GetStandardResponse>(json);
        }

        throw new Exception($"Status code: {response.StatusCode} returned from apprenticeships outer api.");
    }
}