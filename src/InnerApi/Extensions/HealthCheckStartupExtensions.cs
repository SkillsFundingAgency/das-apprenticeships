using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SFA.DAS.Apprenticeships.DataAccess;

namespace SFA.DAS.Apprenticeships.InnerApi.Extensions;

/// <summary>
/// HealthCheckStartupExtensions
/// </summary>
public static class HealthCheckStartupExtensions
{
    /// <summary>
    /// Add health-checks
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDasHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<ApprenticeshipsDataContext>("Sql Health Check");

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

        return app;
    }
}