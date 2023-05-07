using T3.Web.Services.Players.ValueObjects;

namespace T3.Web.Services.Commit.Models;

public record GameView
{
    public PlayerId? InitialServer { get; init; }
    public PlayerId? InitialReceiver { get; init; }
    
    public PlayerId? CurrentServer { get; init; }
    public PlayerId? CurrentReceiver { get; init; }
    
    public Score Points { get; init; }
}