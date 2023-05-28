using T3.Web.Services.Commit.ValueObjects;
using T3.Web.Services.Players.ValueObjects;
using T3.Web.Services.Rules;

namespace T3.Web.Services.Commit.Models;

/// <summary>
///     Describes an event in which the umpire assigns a penalty card to a player (or other coach/advisor).
/// </summary>
public record PenaltyEvent
{
    /// <summary>
    ///     Identifies the offense that was committed. This is only used in the event a penalty needs to be removed as it was
    ///     added by mistake.
    /// </summary>
    public required PenaltyEventId PenaltyEventId { get; init; }

    /// <summary>
    ///     To who the card was assigned.
    /// </summary>
    public required PlayerId PlayerId { get; init; }

    /// <summary>
    ///     The card that was assigned.
    /// </summary>
    public required PenaltyCard Card { get; init; }

    /// <summary>
    ///     A collection of offenses that were committed.
    /// </summary>
    public required Offense[] Offenses { get; init; }
}