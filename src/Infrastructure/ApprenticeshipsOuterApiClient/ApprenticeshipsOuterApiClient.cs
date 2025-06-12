﻿using System.Diagnostics.CodeAnalysis;
using System.Net;
using Newtonsoft.Json;
using SFA.DAS.Learning.Infrastructure.ApprenticeshipsOuterApiClient.Calendar;
using SFA.DAS.Learning.Infrastructure.ApprenticeshipsOuterApiClient.Standards;

namespace SFA.DAS.Learning.Infrastructure.ApprenticeshipsOuterApiClient;

[ExcludeFromCodeCoverage]
public class ApprenticeshipsOuterApiClient : IApprenticeshipsOuterApiClient
{
    private readonly HttpClient _httpClient;

    private const string GetStandardUrl = "TrainingCourses/standards";
    private const string GetAcademicYearUrl = "CollectionCalendar/academicYear";
    private const string ApprenticeshipControllerUrl = "Apprenticeship";
    private const string HandleWithdrawalNotificationsUrl = "handleWithdrawalNotifications";

    public ApprenticeshipsOuterApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<GetStandardResponse> GetStandard(int courseCode)
    {
        var response = await _httpClient.GetAsync($"{GetStandardUrl}/{courseCode}").ConfigureAwait(false);

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

    public async Task<GetAcademicYearsResponse> GetAcademicYear(DateTime searchYear)
    {
        var response = await _httpClient.GetAsync($"{GetAcademicYearUrl}/{searchYear.ToString("yyyy-MM-dd")}").ConfigureAwait(false);

        if (response.StatusCode.Equals(HttpStatusCode.NotFound))
        {
            throw new Exception("Academic year not found.");
        }

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GetAcademicYearsResponse>(json);
        }

        throw new Exception($"Status code: {response.StatusCode} returned from apprenticeships outer api.");
    }

    public async Task HandleWithdrawalNotifications(Guid apprenticeshipKey, HandleWithdrawalNotificationsRequest request, string serviceBearerToken)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ApprenticeshipControllerUrl}/{apprenticeshipKey}/{HandleWithdrawalNotificationsUrl}");
        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

        // This solution adds the bearer token just for this call as it is the only one triggered from the manual call from azure portal to the HttpTrigger function (for withdrawing simplified payments apprentices)
        // It will need to be replaced when we decide on how we want to handle back end triggered actions like this that require authentication
        requestMessage.Headers.Add("Authorization", $"Bearer {serviceBearerToken}");

        var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}