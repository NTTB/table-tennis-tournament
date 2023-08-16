namespace T3.Web.Services.Match;

/// <summary>
///     A match-entity is only used to lookup a match by id.
///     It does not hold any values or knows which games are in the set.
/// </summary>
public record MatchEntity
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
}