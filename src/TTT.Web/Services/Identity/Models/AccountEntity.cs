namespace TTT.Web.Services.Identity.Models;

public record AccountEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}