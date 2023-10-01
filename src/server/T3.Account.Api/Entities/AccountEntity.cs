using Redis.OM.Modeling;

namespace T3.Account.Api.Entities;

[Document]
public class AccountEntity
{
    [Indexed, RedisIdField] public Guid Id { get; init; }
    [Indexed] public required string Username { get; init; }
    public required string PasswordHash { get; set; }
}