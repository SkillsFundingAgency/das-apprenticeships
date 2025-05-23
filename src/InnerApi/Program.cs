using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Models;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.InnerApi.Identity.Authentication;
using SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;
using SFA.DAS.Apprenticeships.Infrastructure.Extensions;
using SFA.DAS.Apprenticeships.InnerApi.Extensions;
using SFA.DAS.Apprenticeships.InnerApi.Services;

namespace SFA.DAS.Apprenticeships.InnerApi;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = ["SFA.DAS.Apprenticeships", "SFA.DAS.Encoding"];
            options.StorageConnectionString = builder.Configuration["ConfigurationStorageConnectionString"];
            options.EnvironmentName = builder.Configuration["EnvironmentName"];
            options.PreFixConfigurationKeys = false;
            options.ConfigurationKeysRawJsonResult = ["SFA.DAS.Encoding"];
        });

        builder.Services.AddApplicationInsightsTelemetry();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Apprenticeships Internal API"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            opt.IncludeXmlComments(xmlPath);
        });

        var applicationSettings = new ApplicationSettings();
        builder.Configuration.Bind(nameof(ApplicationSettings), applicationSettings);
        builder.Services.AddEntityFrameworkForApprenticeships(applicationSettings, NotLocal(builder.Configuration));
        builder.Services.AddSingleton(x => applicationSettings);
        builder.Services.AddQueryServices();
        builder.Services.AddScoped<IPagedLinkHeaderService, PagedLinkHeaderService>();
        builder.Services.AddApprenticeshipsOuterApiClient(applicationSettings.ApprenticeshipsOuterApiConfiguration.BaseUrl, applicationSettings.ApprenticeshipsOuterApiConfiguration.Key);
        builder.Services.ConfigureNServiceBusForSend(applicationSettings.NServiceBusConnectionString.GetFullyQualifiedNamespace());
        builder.Services.AddCommandServices(builder.Configuration).AddEventServices().AddValidators();
        builder.Services.AddDasHealthChecks(applicationSettings);
        builder.Services.AddApiAuthentication(builder.Configuration, builder.Environment.IsDevelopment());
        builder.Services.AddApiAuthorization(builder.Environment.IsDevelopment());

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseDasHealthChecks();
        app.UseMiddleware<BearerTokenMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.MapHealthChecks("/ping");   // Both /ping 
        app.MapHealthChecks("/");       // and / are used for health checks

        app.Run();

        static bool NotLocal(IConfiguration configuration)
        {
            return !configuration!["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}