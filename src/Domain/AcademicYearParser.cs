namespace SFA.DAS.Learning.Domain;

public static class AcademicYearParser
{
    public static DateRange ParseFrom(int academicYear)
    {
        var academicYearString = academicYear.ToString();

        if (academicYearString.Length != 4)
        {
            throw new ArgumentOutOfRangeException(nameof(academicYear));
        }
        
        var startYearDate = new DateTime(year: int.Parse($"20{academicYearString.Substring(0, 2)}"), month: 8, day: 1);
        var endYearDate = new DateTime(year: int.Parse($"20{academicYearString.Substring(2, 2)}"), month: 7, day: 31);

        var totalDays = (int)(endYearDate - startYearDate).TotalDays;

        if (totalDays is > 365 or < 364)
        {
            throw new ArgumentOutOfRangeException(nameof(academicYear));
        }
        
        return new DateRange(startYearDate, endYearDate);
    }
}
