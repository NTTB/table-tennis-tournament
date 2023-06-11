using System.Text.Json.Serialization;

namespace T3.Web.Services.SecretNotes.Entities;

public record SecretNote
{
    // Note: Exist for MongoDB, but should not be send to the client.
    [JsonIgnore]
    public MongoDB.Bson.ObjectId _id { get; set; }
    
    /// <summary>
    /// The content of the secret note.
    /// </summary>
    public SecretNoteContent Content { get; set; }
    
    /// <summary>
    /// The signature that proofs that the encrypted data was created by the user.
    /// </summary>
    public SecretNoteSignature Signature { get; set; }
}