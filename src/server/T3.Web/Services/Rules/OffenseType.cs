namespace T3.Web.Services.Rules;

/// <summary>
/// Describes the codified (as in rules) offense. This data should be retrieved by the client.
/// </summary>
/// <param name="Code">The code used to quickly identify the offense</param>
/// <param name="Description">A description of the offence</param>
/// <param name="DetailsRequired">Whether the umpire is required to provide additional details</param>
public record OffenseType(string Code, string Description, bool DetailsRequired);