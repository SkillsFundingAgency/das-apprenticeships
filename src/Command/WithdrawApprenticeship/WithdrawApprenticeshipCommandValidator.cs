using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Infrastructure.Services;

namespace SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;

public class WithdrawApprenticeshipCommandValidator : IValidator<WithdrawApprenticeshipCommand>
{
    private readonly ISystemClockService _systemClockService;

    public WithdrawApprenticeshipCommandValidator(ISystemClockService systemClockService)
    {
        _systemClockService = systemClockService;    
    }

    //, ApprenticeshipDomainModel? apprenticeship, DateTime currentAcademicYearEnd
    public bool IsValid(WithdrawApprenticeshipCommand command, out string message, params object?[] args)
    {
        message = string.Empty;

        var apprenticeship = args.OfType<ApprenticeshipDomainModel>().FirstOrDefault();
        var currentAcademicYearEnd = args.OfType<DateTime>().FirstOrDefault();

        // Validate if apprenticeship exists
        if (apprenticeship == null)
            return FailwithMessage(out message, $"No apprenticeship found for ULN {command.ULN}");

        if(apprenticeship.LatestEpisode.Ukprn != command.UKPRN) // This check should really be part of authorization, but is currently passedd in as part of the request body
            return FailwithMessage(out message, $"Apprenticeship not found for ULN {command.ULN} and UKPRN {command.UKPRN}");

        // Validate if already withdrawn
        if (apprenticeship.LatestEpisode.LearningStatus == LearnerStatus.Withdrawn)
            return FailwithMessage(out message, $"Apprenticeship already withdrawn for ULN {command.ULN}");

        // Validate Reason
        if (!ValidateReason(command, out message))
            return false;

        // Validate Withdrawal Date
        if (!ValidateWithdrawlDate(command, apprenticeship, currentAcademicYearEnd, out message))
            return false;

        message = string.Empty;

        return true;
    }

    private bool ValidateReason(WithdrawApprenticeshipCommand command, out string message)
    {
        WithdrawReason reason;

        if (!Enum.TryParse<WithdrawReason>(command.Reason, out reason))
        {
            var validReasons = string.Join(", ", Enum.GetNames(typeof(WithdrawReason)));
            return FailwithMessage(out message, $"Invalid reason, possible values are {validReasons}");
        }

        if (reason == WithdrawReason.Other && string.IsNullOrWhiteSpace(command.ReasonText))
            return FailwithMessage(out message, "Reason text is required for 'Other' reason");

        if(reason == WithdrawReason.Other && command.ReasonText.Length > 100)
            return FailwithMessage(out message, "Reason text must be less than 100 characters");

        message = string.Empty;
        return true;
    }

    private bool ValidateWithdrawlDate(WithdrawApprenticeshipCommand command, ApprenticeshipDomainModel apprenticeship, DateTime currentAcademicYearEnd, out string message)
    {
        message = string.Empty;

        if (command.LastDayOfLearning < apprenticeship.StartDate)
            return FailwithMessage(out message, "LastDayOfLearning cannot be before the start date");

        if (command.LastDayOfLearning > apprenticeship.EndDate)
            return FailwithMessage(out message, "LastDayOfLearning cannot be after the planned end date");

        if (command.LastDayOfLearning > currentAcademicYearEnd)
            return FailwithMessage(out message, "LastDayOfLearning cannot be after the end of the current academic year");

        if (command.LastDayOfLearning > DateTime.Now && apprenticeship.StartDate > DateTime.Now)
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
