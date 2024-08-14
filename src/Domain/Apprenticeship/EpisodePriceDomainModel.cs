namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class EpisodePriceDomainModel
    {
        private readonly DataAccess.Entities.Apprenticeship.EpisodePrice _entity;
        public Guid Key => _entity.Key;
        public bool IsDeleted => _entity.IsDeleted; 
        public DateTime StartDate => _entity.StartDate; 
        public DateTime EndDate => _entity.EndDate; 
        public decimal TotalPrice => _entity.TotalPrice;
        public decimal? EndPointAssessmentPrice => _entity.EndPointAssessmentPrice;
        public decimal? TrainingPrice => _entity.TrainingPrice;
        public int FundingBandMaximum => _entity.FundingBandMaximum;

        internal static EpisodePriceDomainModel New(
            DateTime startDate,
            DateTime endDate,
            decimal totalPrice,
            decimal? trainingPrice,
            decimal? endpointAssessmentPrice,
            int fundingBandMaximum)
        {
            return new EpisodePriceDomainModel(new DataAccess.Entities.Apprenticeship.EpisodePrice
            {
                IsDeleted = false,
                StartDate = startDate,
                EndDate = endDate,
                TotalPrice = totalPrice,
                TrainingPrice = trainingPrice,
                EndPointAssessmentPrice = endpointAssessmentPrice,
                FundingBandMaximum = fundingBandMaximum
            });
        }

        public DataAccess.Entities.Apprenticeship.EpisodePrice GetEntity()
        {
            return _entity;
        }

        public static EpisodePriceDomainModel Get(DataAccess.Entities.Apprenticeship.EpisodePrice entity)
        {
            return new EpisodePriceDomainModel(entity);
        }

        public void UpdatePrice(decimal trainingPrice, decimal assessmentPrice, decimal totalPrice)
        {
            _entity.TotalPrice = totalPrice;
            _entity.TrainingPrice = trainingPrice;
            _entity.EndPointAssessmentPrice = assessmentPrice;
        }

        public void UpdateEndDate(DateTime endDate)
        {
            if (endDate <= _entity.StartDate)
            {
                throw new InvalidOperationException($"An {nameof(_entity.EndDate)} of ({endDate:u}) was attempted to be assigned " +
                                                    $"to the EpisodePrice for Key: {_entity.Key} (Episode: {_entity.EpisodeKey}). " +
                                                    $"The {nameof(_entity.EndDate)} on a {nameof(EpisodePriceDomainModel)} must be greater " +
                                                    $"than the existing {nameof(_entity.StartDate)} ({_entity.StartDate:u}).");
            }
            _entity.EndDate = endDate;
        }

        public void UpdateStartDate(DateTime startDate)
        {
            if (startDate >= _entity.EndDate)
            {
                throw new InvalidOperationException($"An {nameof(_entity.StartDate)} of ({startDate:u}) was attempted to be assigned " +
                                                    $"to the EpisodePrice for Key: {_entity.Key} (Episode: {_entity.EpisodeKey}). " +
                                                    $"The {nameof(_entity.StartDate)} on a {nameof(EpisodePriceDomainModel)} must be less " +
                                                    $"than the existing {nameof(_entity.EndDate)} ({_entity.EndDate:u}).");
            }
            _entity.StartDate = startDate;
        }

        private EpisodePriceDomainModel(DataAccess.Entities.Apprenticeship.EpisodePrice entity)
        {
            _entity = entity;
        }
    }
}
