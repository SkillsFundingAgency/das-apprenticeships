namespace SFA.DAS.Apprenticeships.InnerApi.Requests;

/// <summary>
/// Request model for Freezing apprenticeship payments.
/// </summary>
public class FreezeRequest
{
    /// <summary>
    /// Reason for freezing the payments.
    /// </summary>
    public string? Reason { get; set; }
}
