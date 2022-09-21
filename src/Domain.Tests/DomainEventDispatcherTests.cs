using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests
{
    [TestFixture]
    public class DomainEventDispatcherTests
    {
        private Mock<IServiceProvider> _serviceProvider;
        private DomainEventDispatcher _domainEventDispatcher;

        [SetUp]
        public void SetUp()
        {
            _serviceProvider = new Mock<IServiceProvider>();
            _domainEventDispatcher = new DomainEventDispatcher(_serviceProvider.Object);
        }

        [Test]
        public async Task WhenHandlerFoundThenHandleCalled()
        {
            var commandHandler = new Mock<IDomainEventHandler<TestDomainEvent>>();
            _serviceProvider.Setup(x => x.GetService(typeof(IEnumerable<IDomainEventHandler<TestDomainEvent>>))).Returns(new[] { commandHandler.Object });

            var command = new TestDomainEvent();
            await _domainEventDispatcher.Send(command);

            commandHandler.Verify(x => x.Handle(command, It.IsAny<CancellationToken>()));
        }
    }

    public class TestDomainEvent : IDomainEvent
    {}
}
