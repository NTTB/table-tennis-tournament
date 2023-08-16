namespace T3.Web.Services.Timestamp;

public record ClientOffset
{
    public required int Milliseconds { get; init; }
}