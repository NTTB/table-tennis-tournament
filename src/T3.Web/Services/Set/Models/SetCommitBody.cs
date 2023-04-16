namespace T3.Web.Services.Set.Models;
public enum SetCommitBodyType
{
    NoOp,
}

public abstract class SetCommitBody
{
    public SetCommitBodyType Type { get; }
}

public class SetCommitBodyNoOp : SetCommitBody
{
    public SetCommitBodyType Type => SetCommitBodyType.NoOp;
}