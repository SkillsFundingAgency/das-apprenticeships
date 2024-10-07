namespace SFA.DAS.Apprenticeships.Infrastructure.Services;

public interface ISystemClockService
{
    /// <summary>Retrieves the current system time in UTC.</summary>
    DateTimeOffset UtcNow { get; }
}

public class SystemClockService : ISystemClockService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}