namespace T3.Web.Services.Shared;

public interface IDateTimeService
{
    public DateTime Now { get; }
    public DateTime UtcNow { get; }
    public DateTime Today { get; }
}

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Today => DateTime.Today;
}