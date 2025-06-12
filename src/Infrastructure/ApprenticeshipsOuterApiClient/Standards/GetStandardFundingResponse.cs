﻿using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Learning.Infrastructure.ApprenticeshipsOuterApiClient.Standards;

[ExcludeFromCodeCoverage]
public class GetStandardFundingResponse
{
    public int MaxEmployerLevyCap { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public int Duration { get; set; }

    public static implicit operator GetStandardFundingResponse(ApprenticeshipFunding source)
    {
        return new GetStandardFundingResponse
        {
            Duration = source.Duration,
            EffectiveFrom = source.EffectiveFrom,
            EffectiveTo = source.EffectiveTo,
            MaxEmployerLevyCap = source.MaxEmployerLevyCap
        };
    }
}