namespace T3.Web.Services.Commit.Models;

public enum SetCommitBodyType
{
    Invalid,
    NoOp,
    SetScoreChange,
}

public abstract record SetCommitBody
{
    public virtual SetCommitBodyType Type { get; }
}

public record SetCommitBodyNoOp : SetCommitBody
{
    public override SetCommitBodyType Type => SetCommitBodyType.NoOp;
}

public record SetCommitBodySetScoreChange : SetCommitBody
{
    public override SetCommitBodyType Type => SetCommitBodyType.SetScoreChange;
    public Score SetScoreDelta { get; set; }
}