namespace SFA.DAS.Learning.DataAccess;

public interface ISqlAzureIdentityTokenProvider
{
    Task<string> GetAccessTokenAsync();
    string GetAccessToken();
}