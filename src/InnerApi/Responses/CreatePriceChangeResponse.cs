namespace SFA.DAS.Learning.InnerApi.Responses;

/// <summary>
/// Response returned from a Price Change request
/// </summary>
public class CreatePriceChangeResponse
{
    /// <summary>
    /// Status of the Price Change
    /// </summary>
    public string PriceChangeStatus { get; set; } = string.Empty;
}
