namespace SFA.DAS.Learning.Domain.Extensions;

public static class DateTimeExtensions
{
    public static int CalculateAgeAtDate(this DateTime dateOfBirth, DateTime date)
    {
        int age = date.Year - dateOfBirth.Year;

        // Check if the birthday has not yet occurred this year
        if (date < dateOfBirth.AddYears(age))
        {
            age--;
        }

        return age;
    }

    public static DateTime GetLastCensusDateBefore(this DateTime date)
    {
        var lastDayOfMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);

        return date.Date == lastDayOfMonth ? lastDayOfMonth : new DateTime(date.Year, date.Month, 1).AddDays(-1);
    }

    public static DateTime StartOfCurrentAcademicYear(this DateTime date)
    {
        if (date.Month >= 8)
        {
            return new DateTime(date.Year, 8, 1);
        }
        else
        {
            return new DateTime(date.Year - 1, 8, 1);
        }
    }

    public static DateTime EndOfCurrentAcademicYear(this DateTime date)
    {
        if (date.Month >= 8)
        {
            return new DateTime(date.Year + 1, 7, 31);
        }
        else
        {
            return new DateTime(date.Year, 7, 31);
        }
    }

    public static byte ToCalendarMonth(this byte deliveryPeriod)
    {
        if (deliveryPeriod >= 6)
            return (byte)(deliveryPeriod - 5);
        else
            return (byte)(deliveryPeriod + 7);
    }

    public static short ToCalendarYear(this short academicYear, byte deliveryPeriod)
    {
        if (deliveryPeriod >= 6)
            return short.Parse($"20{academicYear.ToString().Substring(2, 2)}");
        else
            return short.Parse($"20{academicYear.ToString().Substring(0, 2)}");
    }

    public static DateTime GetLastDay(this short academicYear, byte deliveryPeriod)
    {
        var calendarYear = academicYear.ToCalendarYear(deliveryPeriod);
        var calendarMonth = deliveryPeriod.ToCalendarMonth();
        var lastDay = DateTime.DaysInMonth(calendarYear, calendarMonth);
        return new DateTime(calendarYear, calendarMonth, lastDay);
    }
}
