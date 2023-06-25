using T3.Web.Services.Players.ValueObjects;

namespace T3.Web.Services.Commit.Models;

public record PlayerView
{
    public required PlayerId PlayerId { get; init; }
    public required string DisplayName { get; init; } 
}