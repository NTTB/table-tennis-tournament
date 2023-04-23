namespace T3.Web.Services.Commit.Models;

public record SetView
{
    public required Score GamesWon { get; init; }
    
    /// <summary>
    /// The home players in this set.
    /// </summary>
    public required PlayerView[] HomePlayers { get; init; }
    
    /// <summary>
    /// The away players in this set.
    /// </summary>
    public required PlayerView[] AwayPlayers { get; init; }
    
    public PlayerView? CurrentServer { get; init; }
    public PlayerView? CurrentReceiver { get; init; }
    
    /// <summary>
    /// Who served first in this set.
    /// </summary>
    public PlayerView? InitialServer { get; init; }
    
    /// <summary>
    /// Who received first in this set.
    /// </summary>
    public PlayerView? InitialReceiver { get; init; }
}