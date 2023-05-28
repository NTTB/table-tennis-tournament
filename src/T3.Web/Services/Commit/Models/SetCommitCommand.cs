using T3.Web.Services.Commit.ValueObjects;
using T3.Web.Services.Players.ValueObjects;

namespace T3.Web.Services.Commit.Models;

public enum SetCommitBodyType
{
    Invalid,
    NoOp,
    SetHomePlayers,
    SetAwayPlayers,
    UpdateSetScore,
    SetCurrentServer,
    SetInitialServer,
    UpdateGameScore,
    AddGame,
    
    AddWatch,
    UpdateWatch,
    RemoveWatch,
    
    /// <summary>
    /// Adds a new penalty event to the set.
    /// </summary>
    AddPenaltyEvent,

    /// <summary>
    /// Removes an penalty event from the set.
    /// </summary>
    RemovePenaltyEvent,
}

public abstract record SetCommitCommand
{
    public virtual SetCommitBodyType Type { get; }
}

public record NoOpCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.NoOp;
}

public record SetHomePlayersCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.SetHomePlayers;
    public PlayerView[] HomePlayers { get; set; }
}

public record SetAwayPlayersCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.SetAwayPlayers;
    public PlayerView[] AwayPlayers { get; set; }
}

public record UpdateSetScoreCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.UpdateSetScore;
    public Score SetScore { get; set; }
}

public record SetCurrentServerCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.SetCurrentServer;
    public int GameIndex { get; set; }
    public PlayerId ServingPlayer { get; set; }
    public PlayerId ReceivingPlayer { get; set; }
}

public record SetInitialServerCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.SetInitialServer;
    public int GameIndex { get; set; }
    public PlayerId ServingPlayer { get; set; }
    public PlayerId ReceivingPlayer { get; set; }
}

public record UpdateGameScoreCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.UpdateGameScore;
    public int GameIndex { get; set; }
    public Score GameScore { get; set; }
}

public record AddGameCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.AddGame;
    public int Position { get; set; } // At which index to add the games. 0 = start, 1 = after first game, etc.
    public int Amount { get; set; } // The amount of games to add
}


public record AddWatchCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.AddWatch;
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

public record UpdateWatchCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.UpdateWatch;
    public WatchId WatchId { get; set; }
    public Timestamp.Models.Timestamp Timestamp { get; set; }
    public WatchState NewState { get; set; }
}

public record RemoveWatchCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.RemoveWatch;
    public WatchId WatchId { get; set; }
}

public record AddPenaltyEventCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.AddPenaltyEvent;
    public PenaltyEvent PenaltyEvent { get; set; }
}

public record RemovePenaltyEventCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.RemovePenaltyEvent;
    public PenaltyEventId PenaltyEventId { get; set; }
}
