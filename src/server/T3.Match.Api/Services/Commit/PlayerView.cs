using T3.Web.Services.Players;

namespace T3.Web.Services.Commit;

public record PlayerView
{
    public required PlayerId PlayerId { get; init; }
    public required string DisplayName { get; init; } 
}