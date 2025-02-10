using System.Collections.ObjectModel;
using SFA.DAS.Apprenticeships.Domain.Extensions;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship;

public class ApprenticeshipDomainModel : AggregateRoot
{
    private readonly DataAccess.Entities.Apprenticeship.Apprenticeship _entity;
    private readonly List<EpisodeDomainModel> _episodes;
    private readonly List<PriceHistoryDomainModel> _priceHistories;
    private readonly List<StartDateChangeDomainModel> _startDateChanges;
    private readonly List<FreezeRequestDomainModel> _freezeRequests;

    public Guid Key => _entity.Key;
    public long ApprovalsApprenticeshipId => _entity.ApprovalsApprenticeshipId;
    public string Uln => _entity.Uln;
    public string FirstName => _entity.FirstName;
    public string LastName => _entity.LastName;
    public DateTime DateOfBirth => _entity.DateOfBirth;
    public IReadOnlyCollection<EpisodeDomainModel> Episodes => new ReadOnlyCollection<EpisodeDomainModel>(_episodes);
    public IReadOnlyCollection<PriceHistoryDomainModel> PriceHistories => new ReadOnlyCollection<PriceHistoryDomainModel>(_priceHistories);
    public IReadOnlyCollection<StartDateChangeDomainModel> StartDateChanges => new ReadOnlyCollection<StartDateChangeDomainModel>(_startDateChanges);
    public IReadOnlyCollection<FreezeRequestDomainModel> FreezeRequests => new ReadOnlyCollection<FreezeRequestDomainModel>(_freezeRequests);
    public DateTime StartDate
    {
        get
        {
            var startDate = AllPrices.MinBy(x => x.StartDate)?.StartDate;
            if (startDate == null)
            {
                throw new InvalidOperationException($"Unexpected error. {nameof(StartDate)} could not be found in the {nameof(ApprenticeshipDomainModel)}.");
            }

            return startDate.Value;
        }
    }

    public DateTime? EndDate => AllPrices.MaxBy(x => x.StartDate)?.EndDate;
    public IEnumerable<EpisodePriceDomainModel> AllPrices => 
        _episodes.SelectMany(x => x.EpisodePrices).Where(x => !x.IsDeleted);
    public EpisodePriceDomainModel LatestPrice
    {
        get
        {
            var latestPrice = AllPrices.MaxBy(x => x.StartDate);
            if (latestPrice == null)
            {
                throw new InvalidOperationException($"Unexpected error. {nameof(LatestPrice)} could not be found in the {nameof(ApprenticeshipDomainModel)}.");
            }

            return latestPrice;
        }
    }
    public EpisodeDomainModel LatestEpisode
    {
        get
        {
            var latestEpisode = _episodes.MaxBy(x => x.EpisodePrices.Where(y => !y.IsDeleted).Max(y => y.StartDate));
            if (latestEpisode == null)
            {
                throw new InvalidOperationException($"Unexpected error. {nameof(LatestEpisode)} could not be found in the {nameof(ApprenticeshipDomainModel)}.");
            }

            return latestEpisode;
        }
    }

    public PriceHistoryDomainModel? PendingPriceChange => _priceHistories.SingleOrDefault(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created);
    public StartDateChangeDomainModel? PendingStartDateChange => _startDateChanges.SingleOrDefault(x => x.RequestStatus == ChangeRequestStatus.Created);

    public int AgeAtStartOfApprenticeship => DateOfBirth.CalculateAgeAtDate(StartDate);

    internal static ApprenticeshipDomainModel New(
        long approvalsApprenticeshipId,
        string uln,
        DateTime dateOfBirth,
        string firstName,
        string lastName,
        string apprenticeshipHashedId)
    {
        return new ApprenticeshipDomainModel(new DataAccess.Entities.Apprenticeship.Apprenticeship
        {
            Key = Guid.NewGuid(),
            ApprovalsApprenticeshipId = approvalsApprenticeshipId,
            Uln = uln,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            ApprenticeshipHashedId = apprenticeshipHashedId
        });
    }

    public static ApprenticeshipDomainModel Get(DataAccess.Entities.Apprenticeship.Apprenticeship entity)
    {
        return new ApprenticeshipDomainModel(entity);
    }

    private ApprenticeshipDomainModel(DataAccess.Entities.Apprenticeship.Apprenticeship entity)
    {
        _entity = entity;
        _episodes = entity.Episodes.Select(EpisodeDomainModel.Get).ToList();
        _priceHistories = entity.PriceHistories.Select(PriceHistoryDomainModel.Get).ToList();
        _startDateChanges = entity.StartDateChanges.Select(StartDateChangeDomainModel.Get).ToList();
        _freezeRequests = entity.FreezeRequests.Select(FreezeRequestDomainModel.Get).ToList();
    }

    public void AddEpisode(
        long ukprn,
        long employerAccountId,
        DateTime startDate,
        DateTime endDate,
        decimal totalPrice,
        decimal? trainingPrice,
        decimal? endpointAssessmentPrice,
        FundingType fundingType, 
        FundingPlatform? fundingPlatform,
        int fundingBandMaximum,
        long? fundingEmployerAccountId, 
        string legalEntityName, 
        long? accountLegalEntityId,
        string trainingCode,
        string? trainingCourseVersion)
    {
        var episode = EpisodeDomainModel.New(
            ukprn,
            employerAccountId,
            fundingType,
            fundingPlatform,
            fundingEmployerAccountId,
            legalEntityName,
            accountLegalEntityId,
            trainingCode,
            trainingCourseVersion); 
        
        episode.AddEpisodePrice(
            startDate,
            endDate,
            totalPrice,
            trainingPrice,
            endpointAssessmentPrice,
            fundingBandMaximum);

        _episodes.Add(episode);
        _entity.Episodes.Add(episode.GetEntity());
    }

    public DataAccess.Entities.Apprenticeship.Apprenticeship GetEntity()
    {
        return _entity;
    }

    public void AddPriceHistory(
        decimal? trainingPrice,
        decimal? assessmentPrice,
        decimal totalPrice,
        DateTime effectiveFromDate,
        DateTime createdDate,
        ChangeRequestStatus? priceChangeRequestStatus,
        string? providerApprovedBy,
        string changeReason,
        string? employerApprovedBy,
        DateTime? providerApprovedDate,
        DateTime? employerApprovedDate,
        ChangeInitiator? initiator)
    {
        if(_priceHistories.Any(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created))
        {
            throw new InvalidOperationException("There is already a pending price change for this apprenticeship.");
        }

        var priceHistory = PriceHistoryDomainModel.New(
            Key,
            trainingPrice,
            assessmentPrice,
            totalPrice,
            effectiveFromDate,
            createdDate,
            priceChangeRequestStatus,
            providerApprovedBy,
            providerApprovedDate,
            changeReason,
            employerApprovedBy,
            employerApprovedDate,
            initiator);
            
        _priceHistories.Add(priceHistory);
        _entity.PriceHistories.Add(priceHistory.GetEntity());
    }

    public PriceHistoryDomainModel ApprovePriceChange(string? userApprovedBy, decimal? trainingPrice, decimal? assessmentPrice, DateTime approvedDate)
    {
        var pendingPriceChange = _priceHistories.SingleOrDefault(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created);

        if (pendingPriceChange == null)
            throw new InvalidOperationException("There is no pending price change to approve for this apprenticeship.");

        if (pendingPriceChange.Initiator == ChangeInitiator.Provider)
        {
            pendingPriceChange.ApproveByEmployer(userApprovedBy, approvedDate);
            UpdatePrices(pendingPriceChange);
        }
        else
        {
            if (trainingPrice == null || assessmentPrice == null)
                throw new InvalidOperationException("Both training and assessment prices must be provided when " +
                                                    "approving an employer-initiated price change request.");

            if (pendingPriceChange.TotalPrice != trainingPrice + assessmentPrice)
                throw new InvalidOperationException($"The total price ({pendingPriceChange.TotalPrice}) for this " +
                                                    $"employer-initiated price change request does not match the sum of the " +
                                                    $"training price ({trainingPrice}) and the assessment price ({assessmentPrice}).");

            pendingPriceChange.ApproveByProvider(userApprovedBy, approvedDate, trainingPrice.Value, assessmentPrice.Value);
            UpdatePrices(pendingPriceChange);
        }

        return pendingPriceChange;
    }

    public PriceHistoryDomainModel ProviderAutoApprovePriceChange()
    {
        var pendingPriceChange = _priceHistories.SingleOrDefault(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created);

        if (pendingPriceChange == null)
            throw new InvalidOperationException("There is no pending price change request to approve");

        if (pendingPriceChange.Initiator != ChangeInitiator.Provider)
            throw new InvalidOperationException($"{nameof(ProviderAutoApprovePriceChange)} is only valid for provider-initiated changes");

        pendingPriceChange.AutoApprove();
        UpdatePrices(pendingPriceChange);
        return pendingPriceChange;
    }

    public void CancelPendingPriceChange()
    {
        if (PendingPriceChange == null)
            throw new InvalidOperationException("There is no pending price change request to cancel");

        PendingPriceChange.Cancel();
    }

    public void RejectPendingPriceChange(string? reason)
    {
        if (PendingPriceChange == null)
            throw new InvalidOperationException("There is no pending price change request to reject");
        PendingPriceChange.Reject(reason);
    }

    public void AddStartDateChange(
        DateTime actualStartDate,
        DateTime plannedEndDate,
        string reason,
        string? providerApprovedBy,
        DateTime? providerApprovedDate,
        string? employerApprovedBy,
        DateTime? employerApprovedDate,
        DateTime createdDate,
        ChangeRequestStatus requestStatus,
        ChangeInitiator? initiator)
    {
        if (_startDateChanges.Any(x => x.RequestStatus == ChangeRequestStatus.Created))
        {
            throw new InvalidOperationException("There is already a pending start date change for this apprenticeship.");
        }

        var startDateChange = StartDateChangeDomainModel.New(this.Key,
            actualStartDate,
            plannedEndDate,
            reason,
            providerApprovedBy,
            providerApprovedDate,
            employerApprovedBy,
            employerApprovedDate,
            createdDate,
            requestStatus,
            initiator);

        _startDateChanges.Add(startDateChange);
        _entity.StartDateChanges.Add(startDateChange.GetEntity());
    }

    public StartDateChangeDomainModel ApproveStartDateChange(string? userApprovedBy)
    {
        var pendingStartDateChange = _startDateChanges.SingleOrDefault(x => x.RequestStatus == ChangeRequestStatus.Created);
        if(pendingStartDateChange == null)
            throw new InvalidOperationException("There is no pending start date request to approve for this apprenticeship.");

        var approver = pendingStartDateChange.GetApprover();
        pendingStartDateChange.Approve(approver, userApprovedBy, DateTime.UtcNow);
        LatestEpisode.UpdatePricesForApprovedStartDateChange(pendingStartDateChange);

       return pendingStartDateChange;
    }

    public void RejectStartDateChange(string? reason)
    {
        if (PendingStartDateChange == null)
            throw new InvalidOperationException("There is no pending start date request to reject for this apprenticeship.");

        PendingStartDateChange.Reject(reason);
    }

    public void CancelPendingStartDateChange()
    {
        if (PendingStartDateChange == null)
            throw new InvalidOperationException("There is no pending start date request to cancel for this apprenticeship.");

        PendingStartDateChange.Cancel();
    }

    public void SetPaymentsFrozen(bool newPaymentsFrozenStatus, string userId, DateTime changeDateTime, string? reason = null)
    {
        if (LatestEpisode.PaymentsFrozen == newPaymentsFrozenStatus)
        {
            throw new InvalidOperationException($"Payments are already {(newPaymentsFrozenStatus ? "frozen" : "unfrozen")} for this apprenticeship: {Key}.");
        }

        LatestEpisode.UpdatePaymentStatus(newPaymentsFrozenStatus); 

        if (newPaymentsFrozenStatus)
        {
            var freezeRequest = FreezeRequestDomainModel.New(_entity.Key, userId, changeDateTime, reason);
            _freezeRequests.Add(freezeRequest);
            _entity.FreezeRequests.Add(freezeRequest.GetEntity());
        }
        else
        {
            var freezeRequest = _freezeRequests.Single(x => !x.Unfrozen);
            freezeRequest.Unfreeze(userId, changeDateTime);
        }
    }

    public void WithdrawApprenticeship(string userId, DateTime lastDateOfLearning, string reason, DateTime changeDateTime)
    {
        var currentEpisode = LatestEpisode;

        var withdrawRequest = WithdrawalRequestDomainModel.New(_entity.Key, currentEpisode.Key, reason, lastDateOfLearning, changeDateTime, userId);
        _entity.WithdrawalRequests.Add(withdrawRequest.GetEntity());

        currentEpisode.Withdraw(userId, lastDateOfLearning);

        if (PendingPriceChange != null)
            CancelPendingPriceChange();

        if(PendingStartDateChange != null)
            CancelPendingStartDateChange();

    }

    private void UpdatePrices(PriceHistoryDomainModel priceChangeRequest)
    {
        if (priceChangeRequest!.TrainingPrice == null || priceChangeRequest!.AssessmentPrice == null)
            throw new InvalidOperationException("Both training and assessment prices must be recorded on the pending request in order to approve it.");

        if (LatestPrice.StartDate == priceChangeRequest.EffectiveFromDate)
        {
            LatestPrice.UpdatePrice(priceChangeRequest.TrainingPrice.Value, priceChangeRequest.AssessmentPrice.Value, priceChangeRequest.TotalPrice);
        }
        else
        {
            LatestEpisode.UpdatePricesForApprovedPriceChange(priceChangeRequest);
        }
    }
}