namespace T3.Web.Services.Timestamp.Models;

public record ServerTimestamp
{
    public required int Year { get; init; }
    public required int DayOfYear { get; init; }
    public required int MillisecondOfDay { get; init; }

    /// <summary>
    ///     Noise is a random number to ensure clients can't guess the server timestamp.
    /// </summary>
    public required int Noise { get; init; }

    public static DateTime ToUtcDateTime(ServerTimestamp src)
    {
        var dst = new DateTime(src.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dst = dst.AddDays(src.DayOfYear);
        dst = dst.AddMilliseconds(src.MillisecondOfDay);
        return dst;
    }
}