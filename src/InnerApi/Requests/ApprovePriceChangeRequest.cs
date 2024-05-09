namespace SFA.DAS.Apprenticeships.InnerApi.Requests;

/// <summary>
/// Object to approve a pending price change request (recorded in the PriceHistory table)
/// </summary>
public class ApprovePriceChangeRequest
{
    /// <summary>
    /// Id of the approver, either the provider or the employer
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Only used when a provider is approving a employer initiated price change
    /// </summary>
    public decimal? TrainingPrice { get; set; }

    /// <summary>
    /// Only used when a provider is approving a employer initiated price change
    /// </summary>
    public decimal? AssessmentPrice { get; set; }
}