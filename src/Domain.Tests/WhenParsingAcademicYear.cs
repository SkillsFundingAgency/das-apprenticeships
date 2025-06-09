using System;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.Learning.Domain.UnitTests;

public class WhenParsingAcademicYear
{
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(78)]
    [TestCase(2426)]
    [TestCase(24256)]
    [TestCase(2424)]
    [TestCase(2423)]
    [TestCase(1921)]
    [TestCase(2125)]
    public void Then_Exception_Is_Thrown_When_AcademicYear_Is_Invalid(int academicYear)
    {
        Action action = () => AcademicYearParser.ParseFrom(academicYear);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestCase(1920, "2019-08-01", "2020-07-31")]
    [TestCase(1011, "2010-08-01", "2011-07-31")]
    [TestCase(2324, "2023-08-01", "2024-07-31")] // 2024 is a leap year
    [TestCase(2425, "2024-08-01", "2025-07-31")] // 2024 is a leap year
    [TestCase(3132, "2031-08-01", "2032-07-31")]
    public void Then_ValidDateRange_Is_Returned(int academicYear, string expectedStart, string expectedEnd)
    {
        var result = AcademicYearParser.ParseFrom(academicYear);

        result.Start.Should().Be(DateTime.Parse(expectedStart));
        result.End.Should().Be(DateTime.Parse(expectedEnd));
    }
}