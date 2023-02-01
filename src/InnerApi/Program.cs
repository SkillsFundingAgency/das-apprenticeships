using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Models;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Apprenticeships.InnerApi
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = new[] {"SFA.DAS.Apprenticeships"};
                options.StorageConnectionString = builder.Configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = builder.Configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });

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
            builder.Services.AddDbContext<ApprenticeshipsDataContext>();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            app.MapHealthChecks("/ping");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}