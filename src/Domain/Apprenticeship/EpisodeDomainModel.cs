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
        public bool PaymentsFrozen => _entity.PaymentsFrozen; //TODO verify if needed
        public IReadOnlyCollection<EpisodePriceDomainModel> EpisodePrices => new ReadOnlyCollection<EpisodePriceDomainModel>(_episodePrices);

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

        internal void AddEpisodePrice(
            DateTime? startDate,
            DateTime endDate,
            decimal totalPrice,
            decimal? trainingPrice,
            decimal? endpointAssessmentPrice,
            int fundingBandMaximum)
        {
            var episodePrice = EpisodePriceDomainModel.New(
                startDate,
                endDate,
                totalPrice,
                trainingPrice,
                endpointAssessmentPrice,
                fundingBandMaximum);
            _episodePrices.Add(episodePrice);
            _entity.Prices.Add(episodePrice.GetEntity());
        }

        private EpisodeDomainModel(Episode entity)
        {
            _entity = entity;
            _episodePrices = entity.Prices.Select(EpisodePriceDomainModel.Get).ToList();
        }

        public Episode GetEntity()
        {
            return _entity;
        }

        public static EpisodeDomainModel Get(Episode entity)
        {
            return new EpisodeDomainModel(entity);
        }
    }
}
