namespace T3.Web.Services.Set.Models;

public record SetEntity
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
}