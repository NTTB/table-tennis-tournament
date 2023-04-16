namespace T3.Web.Services.Identity.Entities;

public record AccountEntity
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    
    /// <summary>
    /// Keys used to sign commits.
    /// </summary>
    public required AccountKey[] Keys { get; set; }
}

public record AccountKey
{
    /// <summary>
    /// When the key was created
    /// </summary>
    public required DateTime CreatedAtUtc { get; set; }
    
    /// <summary>
    /// When the key was revoked, when set (no matter the value) the key is considered revoked.
    /// </summary>
    public required DateTime? RevokedAtUtc { get; set; }
    /// <summary>
    /// Known public keys of the account.
    /// </summary>
    public required string PublicKey { get; set; }
}