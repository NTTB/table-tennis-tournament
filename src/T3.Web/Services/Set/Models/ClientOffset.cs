namespace T3.Web.Services.Set.Models;

public record ClientOffset
{
    public required int Milliseconds { get; init; }
}