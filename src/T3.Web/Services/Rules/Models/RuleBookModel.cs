using T3.Web.Services.Rules.ValueObjects;

namespace T3.Web.Services.Rules.Models;

public record RuleBookModel : RuleBook
{
    public required RulebookId Id { get; init; }
}