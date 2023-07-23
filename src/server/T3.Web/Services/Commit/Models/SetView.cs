namespace T3.Web.Services.Commit.Models;

public record SetView
{
    public required Score GamesWon { get; init; }

    /// <summary>
    /// The home players in this set.
    /// </summary>
    public TeamView? HomeTeam { get; init; }

    /// <summary>
    /// The away players in this set.
    /// </summary>
    public TeamView? AwayTeam { get; init; }

    /// <summary>
    /// The games in this set.
    /// </summary>
    public required GameView[] Games { get; init; }
    
    /// <summary>
    /// The watches in the current set. They are not part of any game.
    /// </summary>
    public required WatchView[] SetWatches { get; init; }
    
    /// <summary>
    /// The penalty events in the current set. They are not part of any game.
    /// </summary>
    public required PenaltyEvent[] PenaltyEvents { get; init; }
}