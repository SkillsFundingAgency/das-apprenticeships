using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;

namespace SFA.DAS.Apprenticeships.DataAccess
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkForApprenticeships(this IServiceCollection services, ApplicationSettings settings, bool connectionNeedsAccessToken)
        {
            services.AddSingleton(new AzureServiceTokenProvider());

            services.AddSingleton<ISqlAzureIdentityTokenProvider, SqlAzureIdentityTokenProvider>();

            services.AddSingleton(provider => new SqlAzureIdentityAuthenticationDbConnectionInterceptor(provider.GetService<ILogger<SqlAzureIdentityAuthenticationDbConnectionInterceptor>>(), provider.GetService<ISqlAzureIdentityTokenProvider>(), connectionNeedsAccessToken));
            services.AddDbContext<ApprenticeshipsDataContext>((provider, options) =>
                options
                    .UseSqlServer(new SqlConnection(settings.DbConnectionString),
                        optionsBuilder => optionsBuilder.CommandTimeout(7200)) //7200=2hours
                    .AddInterceptors(provider.GetRequiredService<SqlAzureIdentityAuthenticationDbConnectionInterceptor>()));
            services.AddScoped(provider => new Lazy<ApprenticeshipsDataContext>(provider.GetService<ApprenticeshipsDataContext>));
            
            return services;
        }
    }

    
}
