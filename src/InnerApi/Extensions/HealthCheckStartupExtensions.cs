using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;
using SFA.DAS.Apprenticeships.DataAccess.Extensions;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;

namespace SFA.DAS.Apprenticeships.InnerApi.Extensions;

/// <summary>
/// HealthCheckStartupExtensions
/// </summary>
public static class HealthCheckStartupExtensions
{
    /// <summary>
    /// Add health-checks
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="appSettings">Application Settings instance.</param>
    /// <returns></returns>
    public static IServiceCollection AddDasHealthChecks(this IServiceCollection services, ApplicationSettings appSettings)
    {
        services
            .AddHealthChecks()
            .AddSqlServer(appSettings.DbConnectionString, beforeOpenConnectionConfigurer: connection =>
            {
                {
                    var conn = DatabaseExtensions.GetSqlConnection(appSettings.DbConnectionString);
                    connection.AccessToken = ((SqlConnection)conn).AccessToken;
                }
            });

        return services;
    }

    /// <summary>
    /// Wires up health checks to endpoints
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseDasHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/info", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = (context, _) =>
            {
                context.Response.ContentType = "application/json";
                var info = new
                {
                    Version = "1.0.0",
                    Name = "Apprenticeships Inner API"
                };
                return context.Response.WriteAsync(JsonSerializer.Serialize(info));
            }
        });

        return app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = (httpContext, report) => httpContext.Response.WriteJsonAsync(new
            {
                report.Status,
                report.TotalDuration,
                Results = report.Entries.ToDictionary(
                    e => e.Key,
                    e => new
                    {
                        e.Value.Status,
                        e.Value.Duration,
                        e.Value.Description,
                        e.Value.Data
                    })
            })
        });
    }
}