namespace T3.Web.Services.Rules;

public record RuleBook
{
    public required string DisplayName { get; init; }
    public required OffenseType[] OffenseTypes { get; init; }
}