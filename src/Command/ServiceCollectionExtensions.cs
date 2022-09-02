﻿using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apprenticeships.Command.Decorators;
using SFA.DAS.Apprenticeships.DataAccess.Repositories;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.ApprovalsOuterApiClient;

namespace SFA.DAS.Apprenticeships.Command
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddCommandHandlers(AddCommandHandlerDecorators)
                .AddScoped<ICommandDispatcher, CommandDispatcher>();

            serviceCollection.AddScoped<IApprenticeshipFactory, ApprenticeshipFactory>();
            serviceCollection.AddPersistenceServices();
            serviceCollection.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            return serviceCollection;
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection serviceCollection, Func<IServiceCollection, IServiceCollection> addDecorators = null)
        {
            // set up the command handlers and command validators
            serviceCollection.Scan(scan =>
            {
                scan.FromAssembliesOf(typeof(ServiceCollectionExtensions))
                    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime();
            });

            if (addDecorators != null)
            {
                serviceCollection = addDecorators(serviceCollection);
            }

            return serviceCollection;
        }

        private static IServiceCollection AddPersistenceServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IApprenticeshipRepository, ApprenticeshipRepository>();
            return serviceCollection;
        }

        private static IServiceCollection AddCommandHandlerDecorators(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .Decorate(typeof(ICommandHandler<>), typeof(CommandHandlerWithUnitOfWork<>));

            return serviceCollection;
        }

        public static IServiceCollection AddApprovalsOuterApiClient(this IServiceCollection serviceCollection, string baseAddress, string key)
        {
            serviceCollection.AddScoped<IApprovalsOuterApiClient, ApprovalsOuterApiClient>(x =>
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(baseAddress);
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                httpClient.DefaultRequestHeaders.Add("X-Version", "1");
                return new ApprovalsOuterApiClient(httpClient);
            });

            return serviceCollection;
        }
    }
}
