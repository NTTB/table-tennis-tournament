namespace T3.Web.Services.Timestamp.Models;

public record ClientOffset
{
    public required int Milliseconds { get; init; }
}