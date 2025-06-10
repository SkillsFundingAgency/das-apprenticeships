using SFA.DAS.Learning.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Learning.DataAccess.Extensions
{
    public static class ApprenticeshipExtensions
    {
        public static Episode GetEpisode(this Apprenticeship apprenticeship)
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

        public static int GetAgeAtStartOfApprenticeship(this Apprenticeship apprenticeship)
        {
            var startDate = apprenticeship.Episodes.SelectMany(e => e.Prices).Where(y => !y.IsDeleted).Min(p => p.StartDate);
            var age = startDate.Year - apprenticeship.DateOfBirth.Year;

            if (startDate < apprenticeship.DateOfBirth.AddYears(age)) age--;

            return age;
        }

        public static DateTime GetStartDate(this Apprenticeship apprenticeship)
        {
            return apprenticeship.Episodes.SelectMany(e => e.Prices).Where(y => !y.IsDeleted).Min(p => p.StartDate);
        }

        public static DateTime GetPlannedEndDate(this Apprenticeship apprenticeship)
        {
            return apprenticeship.Episodes.SelectMany(e => e.Prices).Where(y => !y.IsDeleted).Max(p => p.EndDate);
        }

        /// <summary>
        /// NOTE **** This can be replaced in the future with LastDayOfLearning ****
        /// 
        /// This currently returns the Withdraw date which is the last day of learning when the apprenticeship was withdrawn
        /// 
        /// This is needed for some of the FM36 block details which are assembled in the earnings outer API
        /// At some point an actual LastDayOfLearning field will be added to the apprenticeship entity which will either have 
        /// actual last day of learning on the course or withdrawn date as its value. 
        /// At that time LastDayOfLearning can replace this method
        /// </summary>
        public static DateTime? GetWithdrawnDate(this Apprenticeship apprenticeship)
        {
            if(!apprenticeship.WithdrawalRequests.Any())
                return null;

            return apprenticeship.WithdrawalRequests.Select(x=>x.LastDayOfLearning)?.Max();
        }

        private static Episode? GetLatestActiveEpisode(Apprenticeship apprenticeship)
        {
            var episode = apprenticeship.Episodes
                .MaxBy(x => x.Prices
                    .Where(y => !y.IsDeleted)
                    .Select(y => (DateTime?)y.StartDate).DefaultIfEmpty(null) //Ensures that we return null if there are no active prices
                    .Max());
            return episode;
        }

        private static Episode? GetLatestEpisode(Apprenticeship apprenticeship)
        {
            var episode = apprenticeship.Episodes.MaxBy(x => x.Prices.Max(y => y.StartDate));
            return episode;
        }
    }
}
