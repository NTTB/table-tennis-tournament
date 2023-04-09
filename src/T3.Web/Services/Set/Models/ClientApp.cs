namespace T3.Web.Services.Set.Models;

public record ClientApp
{
    public required string Name { get; init; }
    public required string? Version { get; init; }
    public required string? Url { get; init; }
}