using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkForApprenticeships(this IServiceCollection services)
        {
            return services.AddScoped(p =>
            {
                var settings = p.GetService<IOptions<ApplicationSettings>>();
                var optionsBuilder = new DbContextOptionsBuilder<ApprenticeshipsDataContext>().UseSqlServer(settings.Value.DbConnectionString);
                var dbContext = new ApprenticeshipsDataContext(optionsBuilder.Options);

                return dbContext;
            });
        }
    }
}
