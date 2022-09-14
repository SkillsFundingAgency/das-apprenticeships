using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkForApprenticeships(this IServiceCollection services, ApplicationSettings settings)
        {
            services.AddSingleton(new AzureServiceTokenProvider());

            services.AddSingleton<ISqlAzureIdentityTokenProvider, SqlAzureIdentityTokenProvider>();

            services.AddSingleton(provider => new SqlAzureIdentityAuthenticationDbConnectionInterceptor(provider.GetService<ILogger<SqlAzureIdentityAuthenticationDbConnectionInterceptor>>(), provider.GetService<ISqlAzureIdentityTokenProvider>(), settings.ConnectionNeedsAccessToken));

            services.AddScoped(p =>
            {
                var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>()
                    .UseSqlServer(new SqlConnection(settings.DbConnectionString), optionsBuilder => optionsBuilder.CommandTimeout(7200)) //7200=2hours
                    .AddInterceptors(p.GetRequiredService<SqlAzureIdentityAuthenticationDbConnectionInterceptor>())
                    .Options;
                return new ApprenticeshipsDataContext(options);
            });

            return services.AddScoped(provider => new Lazy<ApprenticeshipsDataContext>(provider.GetService<ApprenticeshipsDataContext>));
        }
    }
}
