using T3.Web.Services.Rules.ValueObjects;

namespace T3.Web.Services.Rules;

public static class RulebookIds
{
    public static RulebookId NttbRulebookId2023 { get; } =
        new() { Value = Guid.Parse("74FB8826-880B-43FF-8598-DAA936694536") };
}