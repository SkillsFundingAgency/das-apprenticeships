namespace SFA.DAS.Apprenticeships.DataAccess;

public interface ISqlAzureIdentityTokenProvider
{
    Task<string> GetAccessTokenAsync();
    string GetAccessToken();
}