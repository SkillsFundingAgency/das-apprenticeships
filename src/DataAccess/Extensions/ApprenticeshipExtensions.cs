using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.DataAccess.Extensions
{
    internal static class ApprenticeshipExtensions
    {
        //todo given this is now looking at future and past episodes to find the relevant one to authorise against, do we want to rename this method? GetRelevantEpisodeForAuthorisation is a bit of a mouthful but more descriptive
        //todo check this can run inside a Linq to entities predicate
        internal static Episode GetCurrentEpisode(this Apprenticeship apprenticeship)
        {
            var episode = apprenticeship.Episodes.Find(x => x.Prices != null && x.Prices.Exists(price => price.StartDate <= DateTime.UtcNow && price.EndDate >= DateTime.UtcNow));

            if (episode == null)
            {
                // if no episode is active for the current date, then there could be an episode for the apprenticeship that is yet to start
                episode = apprenticeship.Episodes.Single(x => x.Prices != null && x.Prices.Exists(price => price.StartDate >= DateTime.UtcNow));
            }

            if (episode == null)
            {
                // if no episode is active or yet to start, then there could be a finished episode for the apprenticeship that is complete
                episode = apprenticeship.Episodes.Single(x => x.Prices != null && x.Prices.Exists(price => price.EndDate <= DateTime.UtcNow));
            }

            if (episode == null)
                throw new InvalidOperationException("No current episode found");

            return episode;
        }
    }
}
