using T3.Web.Services.Players.ValueObjects;

namespace T3.Web.Services.Commit.Models;

public record GameView
{
    public required PlayerId? InitialServer { get; init; }
    public required PlayerId? InitialReceiver { get; init; }

    public required PlayerId? CurrentServer { get; init; }
    public required PlayerId? CurrentReceiver { get; init; }

    public required Score Points { get; init; } = Score.CreateZero();
    
    /// <summary>
    /// The watches in the current game. They are not part of any set.
    /// </summary>
    public required WatchView[] Watches { get; init; }
}