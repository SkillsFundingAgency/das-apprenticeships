using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkForApprenticeships(this IServiceCollection services, ApplicationSettings settings)
        {
            services.AddScoped(p =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApprenticeshipsDataContext>().UseSqlServer(settings.DbConnectionString);
                var dbContext = new ApprenticeshipsDataContext(optionsBuilder.Options);

                return dbContext;
            });

            return services.AddScoped(provider => new Lazy<ApprenticeshipsDataContext>(provider.GetService<ApprenticeshipsDataContext>));
        }
    }
}
