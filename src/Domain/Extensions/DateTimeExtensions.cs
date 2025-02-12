namespace SFA.DAS.Apprenticeships.Domain.Extensions;

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
}
