using Microsoft.AspNetCore.SignalR;
using T3.Web.Services.Commit;
using T3.Web.Services.Commit.Models;
using T3.Web.Services.Identity;
using T3.Web.Services.Set.ValueObjects;

namespace T3.Web.Hubs;

public class SetHub : Hub
{
    private readonly IAccountTokenService _accountTokenService;
    private readonly ILogger<SetHub> _logger;
    private readonly ISetCommitService _setCommitService;

    public SetHub(ISetCommitService setCommitService,
        IAccountTokenService accountTokenService,
        ILogger<SetHub> logger
    )
    {
        _setCommitService = setCommitService;
        _accountTokenService = accountTokenService;
        _logger = logger;
    }

    public async Task Push(SetCommit commit)
    {
        // Store changes
        var identity = _accountTokenService.GetIdentity(Context.User);
        if (identity.AccountId != commit.Header.Author.UserId.Value)
            throw new Exception("AccountId of user does not match author of commit");

        await _setCommitService.Add(commit);

        // Inform clients
        _logger.LogInformation("Sending SetCommitPushed to group {SetId}", SetIdGroup(commit.Header.SetId));
        await Clients.Groups(SetIdGroup(commit.Header.SetId)).SendAsync("SetCommitPushed", commit);
        await Clients.Group(AllSetGroupName()).SendAsync("SetCommitPushed", commit);
    }

    public async Task AddSetWatchAll()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, AllSetGroupName());
    }

    public async Task RemoveSetWatchAll()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, AllSetGroupName());
    }

    public async Task AddSetWatch(SetId setId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, SetIdGroup(setId));
        
        // Send the latest commit.
        var commits = await _setCommitService.GetAll(setId);
        foreach (var commit in commits)
        {
            await Clients.Caller.SendAsync("SetCommitPushed", commit);
        }
    }

    public async Task RemoveSetWatch(SetId setId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, SetIdGroup(setId));
    }

    public async Task<SetCommit[]> GetAllCommits(SetId setId)
    {
        var commits = await _setCommitService.GetAll(setId);
        return commits.ToArray();
    }

    private static string AllSetGroupName()
    {
        return "set-*";
    }

    private static string SetIdGroup(SetId setId)
    {
        return $"set-{setId.Value}";
    }
}