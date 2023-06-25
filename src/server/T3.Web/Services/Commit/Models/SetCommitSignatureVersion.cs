namespace T3.Web.Services.Commit.Models;

public enum SetCommitSignatureVersion
{
    /// <summary>
    /// The Payload will contain a JSON-string of the <see cref="SetCommit"/> without the signature.
    /// </summary>
    V1,
}