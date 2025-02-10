using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Infrastructure.Services;

namespace SFA.DAS.Apprenticeships.Domain.Validators;

#pragma warning disable 8618
public class WithdrawDomainRequest
{
    public long UKPRN { get; set; }
    public string ULN { get; set; }
    public string Reason { get; set; }
    public string ReasonText { get; set; }
    public DateTime LastDayOfLearning { get; set; }
    public string ProviderApprovedBy { get; set; }
}
#pragma warning restore 8618

public enum WithdrawReason
{
    WithdrawFromStart,
    WithdrawDuringLearning,
    WithdrawFromBeta,
    Other
}

public class WithdrawValidator : IValidator<WithdrawDomainRequest>
{
    private readonly ISystemClockService _systemClockService;

    public WithdrawValidator(ISystemClockService systemClockService)
    {
        _systemClockService = systemClockService;
    }

    public bool IsValid(WithdrawDomainRequest request, out string message, params object?[] args)
    {
        message = string.Empty;

        var apprenticeship = args.OfType<ApprenticeshipDomainModel>().FirstOrDefault();
        var currentAcademicYearEnd = args.OfType<DateTime>().FirstOrDefault();

        // Validate if apprenticeship exists
        if (apprenticeship == null)
            return FailwithMessage(out message, $"No apprenticeship found for ULN {request.ULN}");

        if (apprenticeship.LatestEpisode.Ukprn != request.UKPRN) // This check should really be part of authorization, but is currently passedd in as part of the request body
            return FailwithMessage(out message, $"Apprenticeship not found for ULN {request.ULN} and UKPRN {request.UKPRN}");

        // Validate if already withdrawn
        if (apprenticeship.LatestEpisode.LearningStatus == LearnerStatus.Withdrawn)
            return FailwithMessage(out message, $"Apprenticeship already withdrawn for ULN {request.ULN}");

        // Validate Reason
        if (!ValidateReason(request, out message))
            return false;

        // Validate Withdrawal Date
        if (!ValidateWithdrawlDate(request, apprenticeship, currentAcademicYearEnd, out message))
            return false;

        message = string.Empty;

        return true;
    }

    private bool ValidateReason(WithdrawDomainRequest request, out string message)
    {
        WithdrawReason reason;

        if (!Enum.TryParse<WithdrawReason>(request.Reason, out reason))
        {
            var validReasons = string.Join(", ", Enum.GetNames(typeof(WithdrawReason)));
            return FailwithMessage(out message, $"Invalid reason, possible values are {validReasons}");
        }

        if (reason == WithdrawReason.Other && string.IsNullOrWhiteSpace(request.ReasonText))
            return FailwithMessage(out message, "Reason text is required for 'Other' reason");

        if (reason == WithdrawReason.Other && request.ReasonText.Length > 100)
            return FailwithMessage(out message, "Reason text must be less than 100 characters");

        message = string.Empty;
        return true;
    }

    private bool ValidateWithdrawlDate(WithdrawDomainRequest request, ApprenticeshipDomainModel apprenticeship, DateTime currentAcademicYearEnd, out string message)
    {
        message = string.Empty;
        var now = _systemClockService.UtcNow;

        if (request.LastDayOfLearning < apprenticeship.StartDate)
            return FailwithMessage(out message, "LastDayOfLearning cannot be before the start date");

        if (request.LastDayOfLearning > apprenticeship.EndDate)
            return FailwithMessage(out message, "LastDayOfLearning cannot be after the planned end date");

        if (request.LastDayOfLearning > currentAcademicYearEnd)
            return FailwithMessage(out message, "LastDayOfLearning cannot be after the end of the current academic year");

        if (request.LastDayOfLearning > now && apprenticeship.StartDate < now)
            return FailwithMessage(out message, "LastDayOfLearning cannot be in the future unless the start date is in the future");

        message = string.Empty;
        return true;
    }

    private static bool FailwithMessage(out string message, string failReason)
    {
        message = failReason;
        return false;
    }
}