using T3.Web.Services.Players.ValueObjects;

namespace T3.Web.Services.Commit.Models;

public enum SetCommitBodyType
{
    Invalid,
    NoOp,
    SetHomePlayers,
    SetAwayPlayers,
    SetCurrentService,
    SetInitialService,
    UpdateSetScore,
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

public record SetCurrentServiceCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.SetCurrentService;
    public PlayerId ServingPlayer { get; set; }
    public PlayerId ReceivingPlayer { get; set; }
}

public record SetInitialServiceCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.SetInitialService;
    public PlayerId ServingPlayer { get; set; }
    public PlayerId ReceivingPlayer { get; set; }
}

public record UpdateSetScoreCommand : SetCommitCommand
{
    public override SetCommitBodyType Type => SetCommitBodyType.UpdateSetScore;
    public Score SetScore { get; set; }
}
