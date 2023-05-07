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
    AddGame
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
