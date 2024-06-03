﻿namespace SFA.DAS.Apprenticeships.InnerApi.Requests;

/// <summary>
/// Request to approve a pending start date change request
/// </summary>
public class ApproveStartDateChangeRequest
{
    /// <summary>
    /// User Id of the approver, either the provider or the employer
    /// </summary>
    public string UserId { get; set; }
}