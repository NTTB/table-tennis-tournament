using System.Text.Json.Serialization;

namespace T3.Web.Services.Commit.Models;

public record SetCommit
{
    // Note: Exist for MongoDB, but should not be send to the client.
    [JsonIgnore]
    public MongoDB.Bson.ObjectId _id { get; set; }
    
    public SetCommitHeader Header { get; set; }
    public SetCommitCommand[] Commands { get; set; }
    
    /// <summary>
    /// The view after this commit was applied.
    /// It will be provided by the client, but the server can use the body to validate it.
    /// </summary>
    public SetView View { get; set; }

    public SetCommitSignature Signature { get; set; }
}