using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Learning.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.DataAccess.UnitTests;

public class WhenApplyingAuthorisationFilter
{
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
    }

    [Test]
    public void ProviderCallShouldFilterApprenticeships()
    {
        // Arrange
        var apprenticeship = AccountIdAuthorizerTestHelper.BuildApprenticeshipWithAccountId(54321);
        var filteredApprenticeship = AccountIdAuthorizerTestHelper.BuildApprenticeshipWithAccountId();
        var apprenticeships = new List<Apprenticeship>()
        {
            apprenticeship,
            filteredApprenticeship
        }.AsQueryable();
        var idsInClaims = new List<long> { 54321, 98765 };
        var authorizer = AccountIdAuthorizerTestHelper.SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Provider);

        // Act
        var result = authorizer.ApplyAuthorizationFilterOnQueries(apprenticeships);

        // Assert
        result.Should().Contain(apprenticeship);
        result.Should().NotContain(filteredApprenticeship);
    }

    [Test]
    public void EmployerCallShouldFilterApprenticeships()
    {
        // Arrange
        var apprenticeship = AccountIdAuthorizerTestHelper.BuildApprenticeshipWithAccountId(null, 101);
        var filteredApprenticeship = AccountIdAuthorizerTestHelper.BuildApprenticeshipWithAccountId();
        var apprenticeships = new List<Apprenticeship>()
        {
            apprenticeship,
            filteredApprenticeship
        }.AsQueryable();
        var idsInClaims = new List<long> { 101, 98765 };
        var authorizer = AccountIdAuthorizerTestHelper.SetUpAuthorizer(true, idsInClaims, AccountIdClaimsType.Employer);

        // Act
        var result = authorizer.ApplyAuthorizationFilterOnQueries(apprenticeships);

        // Assert
        result.Should().Contain(apprenticeship);
        result.Should().NotContain(filteredApprenticeship);
    }
}