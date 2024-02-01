using SFA.DAS.Apprenticeships.Enums;

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
    public PriceChangeRequestStatus? PriceChangeRequestStatus => _entity.PriceChangeRequestStatus;

    internal static PriceHistoryDomainModel New(Guid apprenticeshipKey,
        decimal? trainingPrice,
        decimal? assessmentPrice,
        decimal totalPrice,
        DateTime effectiveFromDate,
        DateTime createdDate,
        PriceChangeRequestStatus? priceChangeRequestStatus,
        string? providerApprovedBy,
        DateTime? providerApprovedDate)
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
            ProviderApprovedDate = providerApprovedDate
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

    internal static PriceHistoryDomainModel Get(DataAccess.Entities.Apprenticeship.PriceHistory entity)
    {
        return new PriceHistoryDomainModel(entity);
    }
}