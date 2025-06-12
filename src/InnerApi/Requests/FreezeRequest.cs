﻿namespace SFA.DAS.Learning.InnerApi.Requests;

/// <summary>
/// Request model for freezing apprenticeship payments.
/// </summary>
public class FreezeRequest
{
    /// <summary>
    /// Reason for freezing the payments.
    /// </summary>
    public string? Reason { get; set; }
}
