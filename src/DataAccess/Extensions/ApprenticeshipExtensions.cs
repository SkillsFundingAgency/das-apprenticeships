using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.DataAccess.Extensions
{
    internal static class ApprenticeshipExtensions
    {
        internal static Episode GetEpisode(this Apprenticeship apprenticeship)
        {
            var episode = GetLatestActiveEpisode(apprenticeship);

            if (episode == null)
            {
                episode = GetLatestEpisode(apprenticeship);
            }

            if (episode == null)
                throw new InvalidOperationException("No active episode found");

            return episode;
        }

        private static Episode? GetLatestActiveEpisode(Apprenticeship apprenticeship)
        {
            var episode = apprenticeship.Episodes.MaxBy(x => x.Prices.Where(y => !y.IsDeleted).Max(y => y.StartDate));
            return episode;
        }

        private static Episode? GetLatestEpisode(Apprenticeship apprenticeship)
        {
            var episode = apprenticeship.Episodes.MaxBy(x => x.Prices.Max(y => y.StartDate));
            return episode;
        }
    }
}
