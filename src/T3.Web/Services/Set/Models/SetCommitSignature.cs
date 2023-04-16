namespace T3.Web.Services.Set.Models;

/// <summary>
/// The signature contains the data that was signed and the public key of the signer.
/// </summary>
public record SetCommitSignature
{
    /// <summary>
    /// The way that the signature was created.
    /// </summary>
    public required SetCommitSignatureVersion Version { get; init; }
    
    /// <summary>
    /// The data that was signed (and which one parsed should proof SetCommit).
    /// </summary>
    public required string Payload { get; init; }
    
    /// <summary>
    /// The base64 encoded signature.
    /// </summary>
    public required string Signature { get; init; }
    
    /// <summary>
    /// The public key of the signature.
    /// This is so we can reject message before checking if the user is allowed to sign.
    /// </summary>
    public required string PublicKey { get; init; }
}

public enum SetCommitSignatureVersion
{
    /// <summary>
    /// The Payload will contain a JSON-string of the <see cref="SetCommit"/> without the signature.
    /// </summary>
    V1,
}