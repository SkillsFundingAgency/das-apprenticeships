using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apprenticeships.Domain.Validators;

namespace SFA.DAS.Apprenticeships.Domain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var validatorType = typeof(IValidator<>);
            var validators = assembly.GetTypes()
                                     .Where(t => t.GetInterfaces()
                                                  .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType))
                                     .ToList();

            foreach (var validator in validators)
            {
                var interfaceType = validator.GetInterfaces()
                                             .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType);
                services.AddTransient(interfaceType, validator);
            }

            return services;
        }

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
