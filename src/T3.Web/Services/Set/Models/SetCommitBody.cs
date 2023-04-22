namespace T3.Web.Services.Set.Models;
public enum SetCommitBodyType
{
    Invalid,
    NoOp,
    SetScoreChange,
}

public abstract class SetCommitBody
{
    public virtual SetCommitBodyType Type { get; }
}

public class SetCommitBodyNoOp : SetCommitBody
{
    public override SetCommitBodyType Type => SetCommitBodyType.NoOp;
}

public class SetCommitBodySetScoreChange : SetCommitBody
{
    public override SetCommitBodyType Type => SetCommitBodyType.SetScoreChange;
    public Score SetScoreDelta { get; set; }
}