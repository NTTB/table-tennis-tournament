namespace T3.Web.Services.Set.Entities;

/// <summary>
///     A set-entity is only used to lookup a set by id.
///     It does not hold any values or knows which games are in the set.
/// </summary>
public record SetEntity
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
}