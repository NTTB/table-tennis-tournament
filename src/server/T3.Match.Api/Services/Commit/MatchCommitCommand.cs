using T3.Web.Services.Players;

namespace T3.Web.Services.Commit;

public enum MatchCommitBodyType
{
    Invalid,
    NoOp,
    SetHomeTeam,
    SetAwayTeam,
    UpdateSetScore,
    SetCurrentServer,
    SetInitialServer,
    UpdateGameScore,
    AddGame,
    
    AddWatch,
    UpdateWatch,
    RemoveWatch,
    
    /// <summary>
    /// Adds a new penalty event to the match.
    /// </summary>
    AddPenaltyEvent,
    
    /// <summary>
    /// Updates an existing penalty event in the match.
    /// </summary>
    UpdatePenaltyEvent,
    
    /// <summary>
    /// Removes an penalty event from the match.
    /// </summary>
    RemovePenaltyEvent,
}

public abstract record MatchCommitCommand
{
    public virtual MatchCommitBodyType Type { get; }
}

public record NoOpCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.NoOp;
}

public record MatchHomeTeamCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.SetHomeTeam;
    public TeamView? HomeTeam { get; set; }
}

public record MatchAwayTeamCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.SetAwayTeam;
    public TeamView? AwayTeam { get; set; }
}

public record UpdateMatchScoreCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.UpdateSetScore;
    public Score SetScore { get; set; }
}

public record MatchCurrentServerCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.SetCurrentServer;
    public int GameIndex { get; set; }
    public PlayerId ServingPlayer { get; set; }
    public PlayerId ReceivingPlayer { get; set; }
}

public record MatchInitialServerCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.SetInitialServer;
    public int GameIndex { get; set; }
    public PlayerId ServingPlayer { get; set; }
    public PlayerId ReceivingPlayer { get; set; }
}

public record UpdateGameScoreCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.UpdateGameScore;
    public int GameIndex { get; set; }
    public Score GameScore { get; set; }
}

public record AddGameCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.AddGame;
    public int Position { get; set; } // At which index to add the games. 0 = start, 1 = after first game, etc.
    public int Amount { get; set; } // The amount of games to add
}


public record AddWatchCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.AddWatch;
    public WatchId WatchId { get; set; }
    /// <summary>
    /// If null, the watch is added to the set. If not null, the watch is added to the game at the specified index.
    /// </summary>
    public int? GameIndex { get; set; }
    
    /// <summary>
    /// The technical key of the watch. This is used to identify the watch in the UI.
    /// The server will not apply any logic to this key.
    /// </summary>
    public string Key { get; set; }
    
    /// <summary>
    /// The maximum amount of milliseconds the watch is allowed to run.
    /// The watch can go over this limit. It's intended to visualize the progress bar.
    /// </summary>
    public int? MaxMilliseconds { get; set; }
}

public record UpdateWatchCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.UpdateWatch;
    public WatchId WatchId { get; set; }
    public Timestamp.Timestamp Timestamp { get; set; }
    public WatchState NewState { get; set; }
}

public record RemoveWatchCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.RemoveWatch;
    public WatchId WatchId { get; set; }
}

public record AddPenaltyEventCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.AddPenaltyEvent;
    public PenaltyEvent PenaltyEvent { get; set; }
}

public record UpdatePenaltyEventCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.UpdatePenaltyEvent;
    public PenaltyEvent PenaltyEvent { get; set; }
}

public record RemovePenaltyEventCommand : MatchCommitCommand
{
    public override MatchCommitBodyType Type => MatchCommitBodyType.RemovePenaltyEvent;
    public PenaltyEventId PenaltyEventId { get; set; }
}
