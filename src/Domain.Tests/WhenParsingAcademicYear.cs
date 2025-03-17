using System;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests;

public class WhenParsingAcademicYear
{
    [Test]
    public void Then_Exception_Is_Thrown_When_AcademicYear_Is_Null()
    {
        var sut = new AcademicYearParser();
        Action action = () => sut.ParseFrom(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Then_Exception_Is_Thrown_When_AcademicYear_Is_Empty()
    {
        var sut = new AcademicYearParser();
        Action action = () => sut.ParseFrom(string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Then_Exception_Is_Thrown_When_AcademicYear_Is_Invalid()
    {
        var sut = new AcademicYearParser();
        Action action = () => sut.ParseFrom("AAAA");
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
    
    [Test]
    public void Then_Exception_Is_Thrown_When_AcademicYear_Is_More_Than_A_Year()
    {
        var sut = new AcademicYearParser();
        Action action = () => sut.ParseFrom("2025");
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestCase("0001", "2000-09-01", "2001-08-31")]
    [TestCase("1920", "2019-09-01", "2020-08-31")]
    [TestCase("1011", "2010-09-01", "2011-08-31")]
    [TestCase("2425", "2024-09-01", "2025-08-31")]
    public void Then_ValidDateRange_Is_Returned(string academicYear, string expectedStart, string expectedEnd)
    {
        var sut = new AcademicYearParser();
        var result = sut.ParseFrom(academicYear);

        result.Start.Should().Be(DateTime.Parse(expectedStart));
        result.End.Should().Be(DateTime.Parse(expectedEnd));
    }
}