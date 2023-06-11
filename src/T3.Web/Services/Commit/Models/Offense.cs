using T3.Web.Services.Commit.ValueObjects;
using T3.Web.Services.Rules;
using T3.Web.Services.SecretNotes.ValueObjects;

namespace T3.Web.Services.Commit.Models;

public record Offense
{
    /// <summary>
    /// The type of offense that was committed. Can be retrieved from the rulebook or set manually.
    /// </summary>
    public required OffenseType Type { get; init; }
    
    /// <summary>
    /// The id of the note that was created and attached. Due to it being a private note, only a few can retrieve it.
    /// </summary>
    public required SecretNoteId? SecretNoteId { get; init; }
}