using Microsoft.AspNetCore.SignalR;
using T3.Web.Services.Commit;
using T3.Web.Services.Identity;
using T3.Web.Services.Match;

namespace T3.Web.Hubs;

public interface IMatchServerCalls
{
    Task Push(Message<MatchCommit> message);
    Task AddWatchAll();
    Task RemoveWatchAll();
    Task AddWatchOne(MatchId matchId);
    Task RemoveWatchOne(MatchId matchId);
    Task<MatchCommitEntity[]> GetAllCommits(MatchId matchId);
}

public interface IMatchClientCalls
{
    Task CommitPushed(Message<MatchCommit> message);
}

public class MatchHub : Hub<IMatchClientCalls>, IMatchServerCalls
{
    private readonly IAccountTokenService _accountTokenService;
    private readonly ILogger<MatchHub> _logger;
    private readonly IMatchCommitService _matchCommitService;

    public MatchHub(IMatchCommitService matchCommitService,
        IAccountTokenService accountTokenService,
        ILogger<MatchHub> logger
    )
    {
        _matchCommitService = matchCommitService;
        _accountTokenService = accountTokenService;
        _logger = logger;
    }

    public async Task Push(Message<MatchCommit> message)
    {
        // Store changes
        var identity = _accountTokenService.GetIdentity(Context.User);
        if (identity.AccountId != message.Content.Header.Author.UserId.Value)
            throw new Exception("AccountId of user does not match author of commit");

        var entity = new MatchCommitEntity()
        {
            Content = message.Content,
            Signature = message.Signature,
        };
        await _matchCommitService.Add(entity);

        // Inform clients
        _logger.LogInformation("Sending CommitPushed to group {SetId}", MatchGroupName(message.Content.Header.MatchId));
        await Clients.Groups(MatchGroupName(message.Content.Header.MatchId)).CommitPushed(message);
        await Clients.Group(MatchGroupNameAll()).CommitPushed(message);
    }

    public async Task AddWatchAll()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, MatchGroupNameAll());
    }

    public async Task RemoveWatchAll()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, MatchGroupNameAll());
    }

    public async Task AddWatchOne(MatchId matchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, MatchGroupName(matchId));

        // Send the latest commit.
        var commits = await _matchCommitService.GetAll(matchId);
        foreach (var commit in commits.Select(MapToMessage))
        {
            await Clients.Caller.CommitPushed(commit);
        }
    }

    private static Message<MatchCommit> MapToMessage(MatchCommitEntity arg)
    {
        return new Message<MatchCommit>(arg.Content, arg.Signature);
    }

    public async Task RemoveWatchOne(MatchId matchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, MatchGroupName(matchId));
    }

    public async Task<MatchCommitEntity[]> GetAllCommits(MatchId matchId)
    {
        var commits = await _matchCommitService.GetAll(matchId);
        return commits.ToArray();
    }

    private static string MatchGroupNameAll()
    {
        return "match-*";
    }

    private static string MatchGroupName(MatchId matchId)
    {
        return $"match-{matchId.Value}";
    }
}