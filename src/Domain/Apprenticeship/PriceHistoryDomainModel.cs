﻿using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship;

public class PriceHistoryDomainModel
{
    private readonly DataAccess.Entities.Apprenticeship.PriceHistory _entity;

    public Guid Key => _entity.Key;
    public Guid ApprenticeshipKey => _entity.ApprenticeshipKey;
    public decimal? TrainingPrice => _entity.TrainingPrice;
    public decimal? AssessmentPrice => _entity.AssessmentPrice;
    public decimal TotalPrice => _entity.TotalPrice;
    public DateTime EffectiveFromDate => _entity.EffectiveFromDate;
    public string? ProviderApprovedBy => _entity.ProviderApprovedBy;
    public DateTime? ProviderApprovedDate => _entity.ProviderApprovedDate;
    public string? EmployerApprovedBy => _entity.EmployerApprovedBy;
    public DateTime? EmployerApprovedDate => _entity.EmployerApprovedDate;
    public DateTime CreatedDate => _entity.CreatedDate;
    public ChangeRequestStatus? PriceChangeRequestStatus => _entity.PriceChangeRequestStatus;
    public string? ChangeReason => _entity.ChangeReason;
    public ChangeInitiator? Initiator => _entity.Initiator;

    internal static PriceHistoryDomainModel New(Guid apprenticeshipKey,
        decimal? trainingPrice,
        decimal? assessmentPrice,
        decimal totalPrice,
        DateTime effectiveFromDate,
        DateTime createdDate,
        ChangeRequestStatus? priceChangeRequestStatus,
        string? providerApprovedBy,
        DateTime? providerApprovedDate,
        string changeReason,
        string? employerApprovedBy,
        DateTime? employerApprovedDate,
        ChangeInitiator? initiator)
    {
        return new PriceHistoryDomainModel(new DataAccess.Entities.Apprenticeship.PriceHistory
        {
            ApprenticeshipKey = apprenticeshipKey,
            TrainingPrice = trainingPrice,
            AssessmentPrice = assessmentPrice,
            TotalPrice = totalPrice,
            EffectiveFromDate = effectiveFromDate,
            CreatedDate = createdDate,
            PriceChangeRequestStatus = priceChangeRequestStatus,
            ProviderApprovedBy = providerApprovedBy,
            ProviderApprovedDate = providerApprovedDate,
            ChangeReason = changeReason,
            EmployerApprovedBy = employerApprovedBy,
            EmployerApprovedDate = employerApprovedDate,
            Initiator = initiator
        });
    }

    private PriceHistoryDomainModel(DataAccess.Entities.Apprenticeship.PriceHistory entity)
    {
        _entity = entity;
    }

    public DataAccess.Entities.Apprenticeship.PriceHistory GetEntity()
    {
        return _entity;
    }

    public static PriceHistoryDomainModel Get(DataAccess.Entities.Apprenticeship.PriceHistory entity)
    {
        return new PriceHistoryDomainModel(entity);
    }

    public void Cancel()
    {
        _entity.PriceChangeRequestStatus = ChangeRequestStatus.Cancelled;
    }

    public void Reject(string? reason)
    {
        _entity.PriceChangeRequestStatus = ChangeRequestStatus.Rejected;
        _entity.RejectReason = reason;
    }

    // Only applicable when Provider initiates a price change to a lower price
    public void AutoApprove()
    {
        _entity.PriceChangeRequestStatus = ChangeRequestStatus.Approved;
    }

    public void ApproveByEmployer(string? employerApprovedBy, DateTime employerApprovedDate)
    {
        _entity.PriceChangeRequestStatus = ChangeRequestStatus.Approved;
        _entity.EmployerApprovedBy = employerApprovedBy;
        _entity.EmployerApprovedDate = employerApprovedDate;
    }

    public void ApproveByProvider(string? providerApprovedBy, DateTime providerApprovedDate, decimal trainingPrice, decimal assementPrice)
    {
        _entity.PriceChangeRequestStatus = ChangeRequestStatus.Approved;
        _entity.ProviderApprovedBy = providerApprovedBy;
        _entity.ProviderApprovedDate = providerApprovedDate;
        _entity.TrainingPrice = trainingPrice;
        _entity.AssessmentPrice = assementPrice;
    }
}