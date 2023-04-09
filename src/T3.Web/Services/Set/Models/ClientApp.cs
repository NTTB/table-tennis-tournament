namespace T3.Web.Services.Set.Models;

public record ClientApp
{
    public required string Name { get; init; }
    public string? Version { get; init; }
    public string? Url { get; init; }
}