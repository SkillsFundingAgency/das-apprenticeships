namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class EpisodePriceDomainModel
    {
        private readonly DataAccess.Entities.Apprenticeship.EpisodePrice _entity;

        public bool IsDeleted => _entity.IsDeleted; 
        public DateTime? StartDate => _entity.StartDate; 
        public DateTime EndDate => _entity.EndDate; 
        public decimal TotalPrice => _entity.TotalPrice; //TODO verify if needed
        public int FundingBandMaximum => _entity.FundingBandMaximum; //TODO verify if needed

        internal static EpisodePriceDomainModel New(
            DateTime? startDate,
            DateTime endDate,
            decimal totalPrice,
            decimal? trainingPrice,
            decimal? endpointAssessmentPrice,
            int fundingBandMaximum)
        {
            return new EpisodePriceDomainModel(new DataAccess.Entities.Apprenticeship.EpisodePrice
            {
                Key = Guid.NewGuid(),
                IsDeleted = false,
                StartDate = startDate,
                EndDate = endDate,
                TotalPrice = totalPrice,
                TrainingPrice = trainingPrice,
                EndPointAssessmentPrice = endpointAssessmentPrice,
                FundingBandMaximum = fundingBandMaximum
            });
        }

        private EpisodePriceDomainModel(DataAccess.Entities.Apprenticeship.EpisodePrice entity)
        {
            _entity = entity;
        }

        public DataAccess.Entities.Apprenticeship.EpisodePrice GetEntity()
        {
            return _entity;
        }

        internal static EpisodePriceDomainModel Get(DataAccess.Entities.Apprenticeship.EpisodePrice entity)
        {
            return new EpisodePriceDomainModel(entity);
        }
    }
}
