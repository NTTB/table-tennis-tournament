namespace T3.Web.Services.SecretNotes.Entities;

public enum SecretNoteSignatureVersion
{
    V1,
}

public record SecretNoteSignature
{
    public required SecretNoteSignatureVersion Version { get; init; }
    
    /// <summary>
    /// The string that describes the content, must be the same otherwise we consider the key compromised.
    /// </summary>
    public required string Payload { get; init; }

    /// <summary>
    /// The signature that proofs that the encrypted data was created by the user.
    /// Since the wrapping key is included in the payload, the only one that can disclose the unwrapped key
    /// is the user who created it.
    /// </summary>
    public required string Signature { get; init; }

    /// <summary>
    /// The signature that can be used to verify the signature.
    /// </summary>
    public required string PublicKey { get; init; }
}