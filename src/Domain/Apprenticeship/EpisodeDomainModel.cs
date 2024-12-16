using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using System.Collections.ObjectModel;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship;

public class EpisodeDomainModel
{
    private readonly Episode _entity;
    private readonly List<EpisodePriceDomainModel> _episodePrices;
    public Guid Key => _entity.Key;
    public long Ukprn => _entity.Ukprn;
    public long EmployerAccountId => _entity.EmployerAccountId;
    public FundingType FundingType => _entity.FundingType;
    public FundingPlatform? FundingPlatform => _entity.FundingPlatform;
    public long? FundingEmployerAccountId => _entity.FundingEmployerAccountId;
    public string LegalEntityName => _entity.LegalEntityName;
    public long? AccountLegalEntityId => _entity.AccountLegalEntityId;
    public string TrainingCode => _entity.TrainingCode;
    public string TrainingCourseVersion => _entity.TrainingCourseVersion;
    public bool PaymentsFrozen => _entity.PaymentsFrozen;
    public LearnerStatus LearningStatus => Enum.Parse<LearnerStatus>(_entity.LearningStatus);
    public DateTime? LastDayOfLearning => _entity.LastDayOfLearning;

    public IReadOnlyCollection<EpisodePriceDomainModel> EpisodePrices => new ReadOnlyCollection<EpisodePriceDomainModel>(_episodePrices);
    public List<EpisodePriceDomainModel> ActiveEpisodePrices => _episodePrices.Where(x => !x.IsDeleted).ToList();
    public EpisodePriceDomainModel LatestPrice
    {
        get
        {
            var latestPrice = _episodePrices.Where(y => !y.IsDeleted).MaxBy(x => x.StartDate);
            if (latestPrice == null)
            {
                throw new InvalidOperationException($"Unexpected error. {nameof(LatestPrice)} could not be found in the {nameof(EpisodeDomainModel)}.");
            }

            return latestPrice;
        }
    }

    public EpisodePriceDomainModel FirstPrice
    {
        get
        {
            var firstPrice = _episodePrices.Where(y => !y.IsDeleted).MinBy(x => x.StartDate);
            if (firstPrice == null)
            {
                throw new InvalidOperationException($"Unexpected error. {nameof(FirstPrice)} could not be found in the {nameof(EpisodeDomainModel)}.");
            }

            return firstPrice;
        }
    }

    internal static EpisodeDomainModel New(
        long ukprn,
        long employerAccountId,
        FundingType fundingType, 
        FundingPlatform? fundingPlatform,
        long? fundingEmployerAccountId, 
        string legalEntityName, 
        long? accountLegalEntityId,
        string trainingCode,
        string? trainingCourseVersion)
    {
        return new EpisodeDomainModel(new Episode
        {
            Ukprn = ukprn,
            EmployerAccountId = employerAccountId,
            FundingType = fundingType,
            FundingPlatform = fundingPlatform,
            FundingEmployerAccountId = fundingEmployerAccountId,
            LegalEntityName = legalEntityName,
            AccountLegalEntityId = accountLegalEntityId,
            TrainingCode = trainingCode,
            TrainingCourseVersion = trainingCourseVersion,
            PaymentsFrozen = false,
            LearningStatus = LearnerStatus.Active.ToString()
        });
    }

    internal void AddEpisodePrice(
        DateTime startDate,
        DateTime endDate,
        decimal totalPrice,
        decimal? trainingPrice,
        decimal? endpointAssessmentPrice,
        int fundingBandMaximum,
        bool shouldSupersedePreviousPrice = false)
    {
        var newEpisodePrice = EpisodePriceDomainModel.New(
            startDate,
            endDate,
            totalPrice,
            trainingPrice,
            endpointAssessmentPrice,
            fundingBandMaximum);

        if (shouldSupersedePreviousPrice)
        {
            LatestPrice.UpdateEndDate(newEpisodePrice.StartDate.AddDays(-1));
        }

        _episodePrices.Add(newEpisodePrice);
        _entity.Prices.Add(newEpisodePrice.GetEntity());
    }

    internal void UpdatePricesForApprovedPriceChange(PriceHistoryDomainModel priceChangeRequest)
    {
        var endDate = LatestPrice.EndDate;
        var fundingBandMaximum = LatestPrice.FundingBandMaximum;
        DeletePricesStartingAfterDate(priceChangeRequest.EffectiveFromDate);

        var remainingPrices = _entity.Prices.Where(x => !x.IsDeleted).ToList();
        var latestActivePrice = remainingPrices.MaxBy(x => x.StartDate);

        var shouldSupersedePreviousPrice = latestActivePrice != null && latestActivePrice.StartDate < priceChangeRequest.EffectiveFromDate;

        AddEpisodePrice(priceChangeRequest.EffectiveFromDate,
            endDate,
            priceChangeRequest.TotalPrice,
            priceChangeRequest.TrainingPrice,
            priceChangeRequest.AssessmentPrice,
            fundingBandMaximum,
            shouldSupersedePreviousPrice);
    }

    internal void UpdatePricesForApprovedStartDateChange(StartDateChangeDomainModel startDateChangeRequest)
    {
        var latestPrice = LatestPrice;
        DeletePricesEndingBeforeDate(startDateChangeRequest.ActualStartDate);
        DeletePricesStartingAfterDate(startDateChangeRequest.PlannedEndDate);

        if (ActiveEpisodePrices.Count == 0)
        {
            AddEpisodePrice(
                startDateChangeRequest.ActualStartDate,
                startDateChangeRequest.PlannedEndDate,
                latestPrice.TotalPrice,
                latestPrice.TrainingPrice,
                latestPrice.EndPointAssessmentPrice,
                latestPrice.FundingBandMaximum);
        }
        else
        {
            if (FirstPrice.StartDate != startDateChangeRequest.ActualStartDate)
            {
                FirstPrice.UpdateStartDate(startDateChangeRequest.ActualStartDate);
            }

            if (LatestPrice.EndDate != startDateChangeRequest.PlannedEndDate)
            {
                LatestPrice.UpdateEndDate(startDateChangeRequest.PlannedEndDate);
            }
        }
    }

    internal void UpdatePaymentStatus(bool isFrozen)
    {
        _entity.PaymentsFrozen = isFrozen;
    }

    public Episode GetEntity()
    {
        return _entity;
    }

    public static EpisodeDomainModel Get(Episode entity)
    {
        return new EpisodeDomainModel(entity);
    }

    private void DeletePricesStartingAfterDate(DateTime date)
    {
        foreach (var price in _entity.Prices.Where(x => x.StartDate > date && !x.IsDeleted))
        {
            price.IsDeleted = true;
        }
    }

    private void DeletePricesEndingBeforeDate(DateTime date)
    {
        foreach (var price in _entity.Prices.Where(x => x.EndDate < date && !x.IsDeleted))
        {
            price.IsDeleted = true;
        }
    }

    internal void Withdraw(string userId, DateTime lastDateOfLearning)
    {
        _entity.LearningStatus = LearnerStatus.Withdrawn.ToString();
        _entity.LastDayOfLearning = lastDateOfLearning;
    }

    private EpisodeDomainModel(Episode entity)
    {
        _entity = entity;
        _episodePrices = entity.Prices.Select(EpisodePriceDomainModel.Get).ToList();
    }
}

/// <summary>
/// Note there are 2 learner status enums in this repo. (The other is in SFA.DAS.Apprenticeships.Types). The other enum is used for calculated
/// status's and is not persisted. This enum is persisted in the database.
/// </summary>
public enum LearnerStatus
{
    Active,
    Withdrawn
}