using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Models;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.DataAccess;
using SFA.DAS.Learning.Domain;
using SFA.DAS.Learning.Infrastructure.Configuration;
using SFA.DAS.Learning.Infrastructure.Extensions;
using SFA.DAS.Learning.InnerApi.Extensions;
using SFA.DAS.Learning.InnerApi.Identity.Authentication;
using SFA.DAS.Learning.InnerApi.Identity.Authorization;
using SFA.DAS.Learning.InnerApi.Services;
using SFA.DAS.Learning.Queries;

namespace SFA.DAS.Learning.InnerApi;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = ["SFA.DAS.Learning", "SFA.DAS.Encoding"];
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
                Title = "Learning Internal API"
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