namespace T3.Web.Services.Set.Models;

public record Score
{
    public required int Home { get; init; }
    public required int Away { get; init; }
}