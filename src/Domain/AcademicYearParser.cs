namespace SFA.DAS.Apprenticeships.Domain;

public class AcademicYearParser
{
    public DateRange ParseFrom(string academicYear)
    {
        if (string.IsNullOrEmpty(academicYear))
        {
            throw new ArgumentNullException(nameof(academicYear));
        }

        if (academicYear.Length != 4)
        {
            throw new ArgumentOutOfRangeException(nameof(academicYear));
        }

        var isValid = int.TryParse(academicYear, out var _);

        if (!isValid)
        {
            throw new ArgumentOutOfRangeException(nameof(academicYear));
        }

        var startYearDate = new DateTime(year: int.Parse($"20{academicYear.Substring(0, 2)}"), month: 9, day: 1);
        var endYearDate = new DateTime(year: int.Parse($"20{academicYear.Substring(2, 2)}"), month: 8, day: 31);

        if ((endYearDate - startYearDate).TotalDays > 365)
        {
            throw new ArgumentOutOfRangeException(nameof(academicYear));
        }

        return new DateRange(startYearDate, endYearDate);
    }
}