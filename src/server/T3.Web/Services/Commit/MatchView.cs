namespace T3.Web.Services.Commit;

public record MatchView
{
    public required Score GamesWon { get; init; }

    /// <summary>
    /// The home players in this match.
    /// </summary>
    public TeamView? HomeTeam { get; init; }

    /// <summary>
    /// The away players in this match.
    /// </summary>
    public TeamView? AwayTeam { get; init; }

    /// <summary>
    /// The games in this match.
    /// </summary>
    public required GameView[] Games { get; init; }
    
    /// <summary>
    /// The watches in the current match. They are not part of any game.
    /// </summary>
    public required WatchView[] MatchWatches { get; init; }
    
    /// <summary>
    /// The penalty events in the current match. They are not part of any game.
    /// </summary>
    public required PenaltyEvent[] PenaltyEvents { get; init; }
}