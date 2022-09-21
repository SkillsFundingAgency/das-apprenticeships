namespace SFA.DAS.Apprenticeships.Infrastructure;

public interface ISqlAzureIdentityTokenProvider
{
    Task<string> GetAccessTokenAsync();
    string GetAccessToken();
}