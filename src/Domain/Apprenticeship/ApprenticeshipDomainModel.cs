using System.Collections.ObjectModel;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship;

public class ApprenticeshipDomainModel : AggregateRoot
{
    private readonly DataAccess.Entities.Apprenticeship.Apprenticeship _entity;
    private readonly List<ApprovalDomainModel> _approvals;
    private readonly List<PriceHistoryDomainModel> _priceHistories;
    private readonly List<StartDateChangeDomainModel> _startDateChanges;

    public Guid Key => _entity.Key;
    public string TrainingCode => _entity.TrainingCode;
    public string Uln => _entity.Uln;
    public string FirstName => _entity.FirstName;
    public string LastName => _entity.LastName;
    public DateTime DateOfBirth => _entity.DateOfBirth;
    public long EmployerAccountId => _entity.EmployerAccountId;
    public long Ukprn => _entity.Ukprn;
    public IReadOnlyCollection<ApprovalDomainModel> Approvals => new ReadOnlyCollection<ApprovalDomainModel>(_approvals);
    public IReadOnlyCollection<PriceHistoryDomainModel> PriceHistories => new ReadOnlyCollection<PriceHistoryDomainModel>(_priceHistories);
    public IReadOnlyCollection<StartDateChangeDomainModel> StartDateChanges => new ReadOnlyCollection<StartDateChangeDomainModel>(_startDateChanges);

    public int? AgeAtStartOfApprenticeship
    {
        get
        {
            var firstApproval = _approvals.OrderBy(x => x.ActualStartDate).First();
            if (!firstApproval.ActualStartDate.HasValue)
            {
                return null;
            }
            var age = firstApproval.ActualStartDate.Value.Year - DateOfBirth.Year;
            if (firstApproval.ActualStartDate.Value.Date.AddYears(-age) < DateOfBirth.Date)
            {
                age--;
            }

            return age;
        }
    }

    internal static ApprenticeshipDomainModel New(
        string uln,
        string trainingCode,
        DateTime dateOfBirth,
        string firstName,
        string lastName,
        decimal? trainingPrice,
        decimal? endpointAssessmentPrice,
        decimal totalPrice,
        string apprenticeshipHashedId,
        int fundingBandMaximum,
        DateTime? actualStartDate,
        DateTime? plannedEndDate,
        long accountLegalEntityId,
        long ukprn,
        long employerAccountId)
    {
        return new ApprenticeshipDomainModel(new DataAccess.Entities.Apprenticeship.Apprenticeship
        {
            Key = Guid.NewGuid(),
            Uln = uln,
            TrainingCode = trainingCode,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            ApprenticeshipHashedId = apprenticeshipHashedId,
            TrainingPrice = trainingPrice,
            EndPointAssessmentPrice = endpointAssessmentPrice,
            TotalPrice = totalPrice,
            FundingBandMaximum = fundingBandMaximum,
            ActualStartDate = actualStartDate,
            PlannedEndDate = plannedEndDate,
            AccountLegalEntityId = accountLegalEntityId,
            EmployerAccountId = employerAccountId,
            Ukprn = ukprn
        });
    }

    internal static ApprenticeshipDomainModel Get(DataAccess.Entities.Apprenticeship.Apprenticeship entity)
    {
        return new ApprenticeshipDomainModel(entity, false);
    }

    private ApprenticeshipDomainModel(DataAccess.Entities.Apprenticeship.Apprenticeship entity, bool newApprenticeship = true)
    {
        _entity = entity;
        _approvals = entity.Approvals.Select(ApprovalDomainModel.Get).ToList();
        _priceHistories = entity.PriceHistories.Select(PriceHistoryDomainModel.Get).ToList();
        _startDateChanges = entity.StartDateChanges.Select(StartDateChangeDomainModel.Get).ToList();
        if (newApprenticeship)
        {
            AddEvent(new ApprenticeshipCreated(_entity.Key));
        }
    }

    public void AddApproval(long approvalsApprenticeshipId, string legalEntityName, DateTime? actualStartDate, DateTime plannedEndDate, decimal agreedPrice, long? fundingEmployerAccountId, FundingType fundingType, int fundingBandMaximum, DateTime? plannedStartDate, FundingPlatform? fundingPlatform)
    {
        var approval = ApprovalDomainModel.New(approvalsApprenticeshipId, legalEntityName, actualStartDate, plannedEndDate, agreedPrice, fundingEmployerAccountId, fundingType, fundingBandMaximum, plannedStartDate, fundingPlatform);
        _approvals.Add(approval);
        _entity.Approvals.Add(approval.GetEntity());
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

        var priceHistory = PriceHistoryDomainModel.New(this.Key,
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

    public void ApprovePriceChange(string? userApprovedBy, decimal? trainingPrice, decimal? assessmentPrice)
    {
        var pendingPriceChange = _priceHistories.SingleOrDefault(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created);

        if(pendingPriceChange == null)
            throw new InvalidOperationException("There is no pending price change to approve for this apprenticeship.");

        if(pendingPriceChange.Initiator == ChangeInitiator.Provider)
        {
            // Employer Approving
            pendingPriceChange?.Approve(userApprovedBy, DateTime.Now);
            AddEvent(new PriceChangeApproved(_entity.Key, pendingPriceChange.Key, ApprovedBy.Employer));
            return;
        }

        // Provider Approving
        if (trainingPrice == null || assessmentPrice == null)
            throw new InvalidOperationException("Both training and assessment prices must be provided when approving an employer-initiated price change.");

        if (pendingPriceChange.TotalPrice != trainingPrice + assessmentPrice)
            throw new InvalidOperationException($"The total price ({pendingPriceChange.TotalPrice}) does not match the sum of the training price ({pendingPriceChange.TrainingPrice}) and the assessment ({pendingPriceChange.AssessmentPrice}) price.");

        pendingPriceChange?.Approve(userApprovedBy, DateTime.Now, trainingPrice.Value, assessmentPrice.Value);
        AddEvent(new PriceChangeApproved(_entity.Key, pendingPriceChange.Key, ApprovedBy.Provider));
    }

    /// <summary>
    /// If provider initiates a price change at a lower price than the current price, 
    /// then the employer does not need to approve the price change and its status can be set to Approved.
    /// </summary>
    public void ProviderSelfApprovePriceChange()
    {
        var pendingPriceChange = _priceHistories.SingleOrDefault(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created);

        if (pendingPriceChange == null)
            throw new InvalidOperationException("There is no pending price change request to approve");

        if (pendingPriceChange.Initiator != ChangeInitiator.Provider)
            throw new InvalidOperationException($"{nameof(ProviderSelfApprovePriceChange)} is only valid for provider-initiated changes");

        pendingPriceChange?.Approve();
        AddEvent(new PriceChangeApproved(_entity.Key, pendingPriceChange!.Key, ApprovedBy.Provider));
    }

    public void CancelPendingPriceChange()
    {
        var pendingPriceChange = _priceHistories.Single(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created);
        pendingPriceChange.Cancel();
    }

    public void RejectPendingPriceChange(string? reason)
    {
        var pendingPriceChange = _priceHistories.Single(x => x.PriceChangeRequestStatus == ChangeRequestStatus.Created);
        pendingPriceChange.Reject(reason);
    }

    public void AddStartDateChange(
        DateTime actualStartDate,
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
    public void ApproveStartDateChange(string? userApprovedBy)
    {
        var pendingStartDateChange = _startDateChanges.SingleOrDefault(x => x.RequestStatus == ChangeRequestStatus.Created);
        if(pendingStartDateChange == null)
            throw new InvalidOperationException("There is no pending start date request to approve for this apprenticeship.");

        var approver = pendingStartDateChange.Initiator == ChangeInitiator.Employer ? ApprovedBy.Provider : ApprovedBy.Employer;
        pendingStartDateChange.Approve(approver, userApprovedBy, DateTime.UtcNow);
        _entity.ActualStartDate = pendingStartDateChange.ActualStartDate;

        AddEvent(new StartDateChangeApproved(_entity.Key, pendingStartDateChange!.Key, approver));
    }
}