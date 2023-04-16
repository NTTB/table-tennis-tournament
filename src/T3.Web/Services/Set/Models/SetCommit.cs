using MongoDB.Bson.Serialization.Attributes;

namespace T3.Web.Services.Set.Models;

public record SetCommit
{
    public required SetCommitHeader Header { get; init; }
    public required SetCommitBody Body { get; init; }
    
    /// <summary>
    /// The view after this commit was applied.
    /// It will be provided by the client, but the server can use the body to validate it.
    /// </summary>
    public required SetView View { get; init; }

    public required SetCommitSignature Signature { get; init; }
}