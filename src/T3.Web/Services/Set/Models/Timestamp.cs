namespace T3.Web.Services.Set.Models;

public record Timestamp
{
    public required ServerTimestamp ServerTimestamp { get; init; }
    public required ClientOffset ClientOffset { get; init; }
}