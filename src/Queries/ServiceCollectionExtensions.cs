using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeships;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Queries
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .Scan(scan =>
                {
                    scan.FromExecutingAssembly()
                        .AddClasses(classes => classes.AssignableTo(typeof(IQuery)))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime();

                    scan.FromAssembliesOf(typeof(GetApprenticeshipsQueryHandler))
                        .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime();
                })
                .AddScoped<IApprenticeshipQueryRepository, ApprenticeshipQueryRepository>()
                .AddScoped<IApprovalQueryRepository, ApprovalQueryRepository>()
                .AddScoped<IQueryDispatcher, QueryDispatcher>();

            return serviceCollection;
        }
    }
}