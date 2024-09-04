using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.DataAccess.Extensions
{
    public static class ApprenticeshipExtensions
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

        public static int GetAgeAtStartOfApprenticeship(this Apprenticeship apprenticeship)
        {
            var startDate = apprenticeship.Episodes.SelectMany(e => e.Prices).Min(p => p.StartDate);
            var age = startDate.Year - apprenticeship.DateOfBirth.Year;

            if (startDate < apprenticeship.DateOfBirth.AddYears(age)) age--;

            return age;
        }

        public static DateTime GetStartDate(this Apprenticeship apprenticeship)
        {
            return apprenticeship.Episodes.SelectMany(e => e.Prices).Min(p => p.StartDate);
        }

        public static DateTime GetPlannedEndDate(this Apprenticeship apprenticeship)
        {
            return apprenticeship.Episodes.SelectMany(e => e.Prices).Max(p => p.EndDate);
        }
    }
}
