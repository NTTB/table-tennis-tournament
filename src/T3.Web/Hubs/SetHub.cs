using Microsoft.AspNetCore.SignalR;
using T3.Web.Services.Set;
using T3.Web.Services.Set.Models;

namespace T3.Web.Hubs;

public class SetHub : Hub
{
    private readonly ISetCommitService _setCommitService;
    private readonly ILogger<SetHub> _logger;

    public SetHub(ISetCommitService setCommitService, ILogger<SetHub> logger)
    {
        _setCommitService = setCommitService;
        _logger = logger;
    }
    
    public async Task Push(SetCommit commit)
    {
        // Store changes
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
        _logger.LogInformation("Adding connection {ConnectionId} to group {SetId}", Context.ConnectionId, SetIdGroup(setId));
        await Groups.AddToGroupAsync(Context.ConnectionId, SetIdGroup(setId));
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
    
    private static string AllSetGroupName() => "set-*";
    private static string SetIdGroup(SetId setId) => $"set-{setId.Value}";
}