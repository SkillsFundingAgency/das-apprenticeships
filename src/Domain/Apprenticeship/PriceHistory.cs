using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship;

public class PriceHistory
{
    private readonly PriceHistoryModel _model;

    public Guid Key => _model.ApprenticeshipKey;
    public Guid ApprenticeshipKey => _model.ApprenticeshipKey;
    public long ApprenticeshipId => _model.ApprenticeshipId;
    public decimal? TrainingPrice => _model.TrainingPrice;
    public decimal? AssessmentPrice => _model.AssessmentPrice;
    public decimal TotalPrice => _model.TotalPrice;
    public DateTime EffectiveFromDate => _model.EffectiveFromDate;
    public string? ProviderApprovedBy => _model.ProviderApprovedBy;
    public DateTime? ProviderApprovedDate => _model.ProviderApprovedDate;
    public string? EmployerApprovedBy => _model.EmployerApprovedBy;
    public DateTime? EmployerApprovedDate => _model.EmployerApprovedDate;
    public DateTime CreatedDate => _model.CreatedDate;
    public PriceChangeRequestStatus? PriceChangeRequestStatus => _model.PriceChangeRequestStatus;

    internal static PriceHistory New(
        Guid apprenticeshipKey,
        decimal? trainingPrice,
        decimal? assessmentPrice,
        decimal totalPrice,
        DateTime effectiveFromDate,
        DateTime createdDate,
        PriceChangeRequestStatus? priceChangeRequestStatus)
    {
        return new PriceHistory(new PriceHistoryModel
        {
            Key = Guid.NewGuid(),
            ApprenticeshipKey = apprenticeshipKey,
            TrainingPrice = trainingPrice,
            AssessmentPrice = assessmentPrice,
            TotalPrice = totalPrice,
            EffectiveFromDate = effectiveFromDate,
            CreatedDate = createdDate,
            PriceChangeRequestStatus = priceChangeRequestStatus
        });
    }

    private PriceHistory(PriceHistoryModel model)
    {
        _model = model;
    }

    public PriceHistoryModel GetModel()
    {
        return _model;
    }

    internal static PriceHistory Get(PriceHistoryModel priceHistoryModel)
    {
        return new PriceHistory(priceHistoryModel);
    }
}