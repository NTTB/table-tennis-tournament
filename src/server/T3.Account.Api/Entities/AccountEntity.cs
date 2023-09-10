namespace T3.Account.Api.Entities;

public record AccountEntity(Guid Id, string Username, string PasswordHash);