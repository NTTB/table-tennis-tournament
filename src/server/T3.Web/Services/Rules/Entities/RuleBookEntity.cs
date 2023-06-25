namespace T3.Web.Services.Rules.Entities;

/// <summary>
/// A rule book entity contains all the rules
/// </summary>
public record RuleBookEntity : RuleBook
{
    /// <summary>
    /// The primary key of the rule book in the mongo database.
    /// </summary>
    public Guid Id { get; set; }
}