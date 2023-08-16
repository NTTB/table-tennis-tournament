using T3.Web.Services.Players;

namespace T3.Web.Services.Commit;

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
    ///     When the penalty event occured.
    /// </summary>
    public required Timestamp.Timestamp Timestamp { get; init; }

    /// <summary>
    ///     To who the card was assigned.
    /// </summary>
    public required PlayerId PlayerId { get; init; }

    /// <summary>
    ///     The card that was assigned.
    /// </summary>
    public required PenaltyCard Card { get; init; }
    
    // TODO: We need to know in which game/set this was given.
}