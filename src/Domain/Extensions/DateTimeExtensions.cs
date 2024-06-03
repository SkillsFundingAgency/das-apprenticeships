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
}
