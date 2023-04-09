namespace T3.Web.Services.Set.Models;

public record SetCommit
{
    public required SetCommitHeader Header { get; init; }
    public required ISetCommitBody Body { get; init; }
    /// <summary>
    /// The view after this commit was applied.
    /// It will be provided by the client, but the server can use the body to validate it.
    /// </summary>
    public required SetView View { get; init; }

    public required SetCommitSignature Signature { get; init; }
}

public record SetView
{
    public required Score GamesWon { get; init; }
}

public record SetCommitSignature
{
    public required string Value { get; init; }
}

public record Score(int Home, int Away);

public enum SetCommitBodyType
{
    NoOp,
}

public interface ISetCommitBody
{
    public SetCommitBodyType Type { get; }
}

public class SetCommitBodyNoOp : ISetCommitBody
{
    public SetCommitBodyType Type => SetCommitBodyType.NoOp;
}