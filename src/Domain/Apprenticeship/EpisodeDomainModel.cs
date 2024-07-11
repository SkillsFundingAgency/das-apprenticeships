using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using System.Collections.ObjectModel;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class EpisodeDomainModel
    {
        private readonly Episode _entity;
        private readonly List<EpisodePriceDomainModel> _episodePrices;
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
        public IReadOnlyCollection<EpisodePriceDomainModel> EpisodePrices => new ReadOnlyCollection<EpisodePriceDomainModel>(_episodePrices);
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
                Key = Guid.NewGuid(),
                Ukprn = ukprn,
                EmployerAccountId = employerAccountId,
                FundingType = fundingType,
                FundingPlatform = fundingPlatform,
                FundingEmployerAccountId = fundingEmployerAccountId,
                LegalEntityName = legalEntityName,
                AccountLegalEntityId = accountLegalEntityId,
                TrainingCode = trainingCode,
                TrainingCourseVersion = trainingCourseVersion,
                PaymentsFrozen = false
            });
        }

        internal EpisodePriceDomainModel AddEpisodePrice(
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
            _episodePrices.Add(newEpisodePrice);
            _entity.Prices.Add(newEpisodePrice.GetEntity());

            if (shouldSupersedePreviousPrice)
            {
                LatestPrice.EndEpisodePrice(newEpisodePrice.StartDate.AddDays(-1));
            }

            return newEpisodePrice;
        }

        internal AmendedPrices UpdatePricesForApprovedPriceChange(PriceHistoryDomainModel priceChangeRequest)
        {
            var endDate = LatestPrice.EndDate;
            var fundingBandMaximum = LatestPrice.FundingBandMaximum;
            var deletedPrices = DeletePricesAfterDate(priceChangeRequest.EffectiveFromDate);
            var newEpisode = AddEpisodePrice(priceChangeRequest.EffectiveFromDate,
                endDate,
                priceChangeRequest.TotalPrice,
                priceChangeRequest.TrainingPrice,
                priceChangeRequest.AssessmentPrice,
                fundingBandMaximum,
                true);
            
            return new AmendedPrices(newEpisode.GetEntity().Key, _entity.Key, deletedPrices.ToList());
        }

        public Episode GetEntity()
        {
            return _entity;
        }

        public static EpisodeDomainModel Get(Episode entity)
        {
            return new EpisodeDomainModel(entity);
        }

        //TODO IS THIS THE BEST WAY OF PASSING AROUND THIS DATA?
        public class AmendedPrices
        {
            public AmendedPrices(Guid latestPriceKey, Guid episodeKey, List<Guid> deletedPriceKeys)
            {
                DeletedPriceKeys = deletedPriceKeys;
                LatestPriceKey = latestPriceKey;
                EpisodeKey = episodeKey;
            }

            public Guid EpisodeKey { get; set; }
            public List<Guid> DeletedPriceKeys { get; set; }
            public Guid LatestPriceKey { get; set; }
        }

        private IEnumerable<Guid> DeletePricesAfterDate(DateTime dateToDeletePricesAfter)
        {
            foreach (var price in _entity.Prices.Where(x => x.StartDate > dateToDeletePricesAfter && !x.IsDeleted))
            {
                price.IsDeleted = true;
                yield return price.Key;
            }
        }

        private EpisodeDomainModel(Episode entity)
        {
            _entity = entity;
            _episodePrices = entity.Prices.Select(EpisodePriceDomainModel.Get).ToList();
        }
    }
}
