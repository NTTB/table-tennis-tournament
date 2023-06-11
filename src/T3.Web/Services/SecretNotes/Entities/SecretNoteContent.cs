using T3.Web.Services.Identity.ValueObjects;
using T3.Web.Services.SecretNotes.ValueObjects;

namespace T3.Web.Services.SecretNotes.Entities;

public enum SecretNotePayloadType
{
    /// <summary>
    /// No encryption is used.
    /// </summary>
    Plain,
}

public abstract record SecretNoteContent
{
    /// <summary>
    /// The type of note
    /// </summary>
    public virtual SecretNotePayloadType Type { get; }

    /// <summary>
    /// An unique id with which the secret note can be identified.
    /// </summary>
    public SecretNoteId Id { get; set; }

    /// <summary>
    /// The version of the secret note. This is because notes can be updated.
    /// </summary>
    public SecretNoteVersionId VersionId { get; set; }


    /// <summary>
    /// When the commit was created according to the client.
    /// </summary>
    public Timestamp.Models.Timestamp CreatedAt { get; set; }

    /// <summary>
    /// The original user who created the secret note. This is also the user holds the wrapped key to decrypt the data.
    /// </summary>
    public UserId UserId { get; set; }
}

public record SecretNoteContentPlain : SecretNoteContent
{
    public override SecretNotePayloadType Type => SecretNotePayloadType.Plain;

    /// <summary>
    /// The content of the secret note, which in this case is plain text.
    /// </summary>
    public string Content { get; set; }
}