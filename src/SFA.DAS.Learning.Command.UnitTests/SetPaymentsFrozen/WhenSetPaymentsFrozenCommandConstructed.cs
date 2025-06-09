using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Learning.Command.SetPaymentsFrozen;

namespace SFA.DAS.Learning.Command.UnitTests.SetPaymentsFrozen;

public class WhenSetPaymentsFrozenCommandConstructed
{
    [TestCase(SetPayments.Freeze, true)]
    [TestCase(SetPayments.Unfreeze, false)]
    public void Then_NewPaymentsFrozenStatus_MatchesExpectedValue(SetPayments setPayments, bool expectedFrozenStatus)
    {
        // Arrange
        var apprenticeshipKey = Guid.NewGuid();
        var userId = "user123";
        
        // Act
        var command = new SetPaymentsFrozenCommand(apprenticeshipKey, userId, setPayments);

        // Assert
        command.NewPaymentsFrozenStatus.Should().Be(expectedFrozenStatus);
    }
}
