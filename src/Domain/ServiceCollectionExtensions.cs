using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Apprenticeships.Domain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan =>
                {
                    scan.FromAssembliesOf(typeof(ServiceCollectionExtensions))
                        .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime();
                })
                .AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            return serviceCollection;
        }
    }
}
