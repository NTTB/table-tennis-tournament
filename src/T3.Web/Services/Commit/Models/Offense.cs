using T3.Web.Services.Rules;

namespace T3.Web.Services.Commit.Models;

public record Offense
{
    /// <summary>
    /// The type of offense that was committed. Can be retrieved from the rulebook or set manually.
    /// </summary>
    public required OffenseType Type { get; init; }
    
    /// <summary>
    /// Extra details about the offense. Can always be provided, but is only required when the type says it is required.
    /// Example: "The player threw the bat in the direction of the umpire."
    /// </summary>
    public required string? Details { get; init; }
}