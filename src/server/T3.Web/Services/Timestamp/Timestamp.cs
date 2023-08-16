namespace T3.Web.Services.Timestamp;

public record Timestamp
{
    public required ServerTimestamp ServerTimestamp { get; init; }
    public required ClientOffset ClientOffset { get; init; }
}