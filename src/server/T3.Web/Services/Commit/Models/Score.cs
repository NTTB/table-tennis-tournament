namespace T3.Web.Services.Commit.Models;

public record Score
{
    public int Home { get; set; }
    public int Away { get; set; }
    
    public static Score CreateZero() => new Score { Home = 0, Away = 0 }; 
}