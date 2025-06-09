using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.Learning.Command;

namespace SFA.DAS.Apprenticeships.Command.UnitTests
{
    [TestFixture]
    public class CommandDispatcherTests
    {
        private Mock<IServiceProvider> _serviceProvider;
        private CommandDispatcher _commandDispatcher;

        [SetUp]
        public void SetUp()
        {
            _serviceProvider = new Mock<IServiceProvider>();
            _commandDispatcher = new CommandDispatcher(_serviceProvider.Object);
        }

        [Test]
        public void WhenHandlerFoundThenHandleCalled()
        {
            var commandHandler = new Mock<ICommandHandler<TestCommand>>();
            _serviceProvider.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand>))).Returns(commandHandler.Object);

            var command = new TestCommand();
            _commandDispatcher.Send(command);

            commandHandler.Verify(x => x.Handle(command, It.IsAny<CancellationToken>()));
        }

        [Test]
        public void WhenHandlerNotFoundThenExceptionThrown()
        {
            _serviceProvider.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand>))).Returns(null);

            var command = new TestCommand();
            Assert.Throws<CommandDispatcherException>(() => _commandDispatcher.Send(command));
        }
    }

    public class TestCommand : ICommand
    {}
}
