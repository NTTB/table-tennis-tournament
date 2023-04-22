using System.Text.Json.Serialization;

namespace T3.Web.Services.Set.Models;

public class SetCommit
{
    // Note: Exist for MongoDB, but should not be send to the client.
    [JsonIgnore]
    public MongoDB.Bson.ObjectId _id { get; set; }
    
    public SetCommitHeader Header { get; set; }
    public SetCommitBody Body { get; set; }
    
    /// <summary>
    /// The view after this commit was applied.
    /// It will be provided by the client, but the server can use the body to validate it.
    /// </summary>
    public SetView View { get; set; }

    public SetCommitSignature Signature { get; set; }
}