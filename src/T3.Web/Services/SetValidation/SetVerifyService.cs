using System.Text.Json;
using T3.Web.Services.Commit;
using T3.Web.Services.Commit.Models;
using T3.Web.Services.Commit.ValueObjects;
using T3.Web.Services.SetValidation.Models;
using T3.Web.Services.Timestamp;

namespace T3.Web.Services.SetValidation;

public interface ISetCommitValidateService
{
    Task<SetCommitValidationResult> ValidateCommit(CommitId commitId);
}

public class SetCommitValidateService : ISetCommitValidateService
{
    private readonly ISetCommitService _setCommitService;
    private readonly ITimestampService _timestampService;

    public SetCommitValidateService(ISetCommitService setCommitService, ITimestampService timestampService)
    {
        _setCommitService = setCommitService;
        _timestampService = timestampService;
    }

    public async Task<SetCommitValidationResult> ValidateCommit(CommitId commitId)
    {
        var lastCommit = await _setCommitService.GetById(commitId);
        var allCommits = (await _setCommitService.GetAll(lastCommit.Header.SetId)).ToArray();

        Stack<SetCommit> commits = new Stack<SetCommit>();

        var currentCommit = lastCommit;
        while (currentCommit != null)
        {
            commits.Push(currentCommit);
            currentCommit = allCommits.SingleOrDefault(x => x.Header.CommitId == currentCommit.Header.PreviousCommitId);
        }

        var allInvalidViews = new List<IncorrectViews>();
        await foreach (var view in  GetInvalidViews(commits))
        {
            allInvalidViews.Add(view);
        }
        
        return new SetCommitValidationResult(
            Valid: !allInvalidViews.Any(),
            InvalidViews: allInvalidViews.ToArray()
        );
    }

    private async IAsyncEnumerable<IncorrectViews> GetInvalidViews(Stack<SetCommit> commits)
    {
        var calculatedView = new SetView
        {
            GamesWon = Score.CreateZero(),
            HomePlayers = Array.Empty<PlayerView>(),
            AwayPlayers = Array.Empty<PlayerView>(),
            Games = Array.Empty<GameView>(),
            SetWatches = Array.Empty<WatchView>(),
            PenaltyEvents = Array.Empty<PenaltyEvent>()
        };

        foreach (var commit in commits)
        {
            calculatedView = await ApplyCommands(calculatedView, commit.Commands);

            // Due to view having arrays, we need to serialize it to compare it.
            var calculatedViewJson = JsonSerializer.Serialize(calculatedView);
            var actualViewJson = JsonSerializer.Serialize(commit.View);

            if (calculatedViewJson == actualViewJson) continue;

            yield return new IncorrectViews(commit.View, calculatedView);
        }
    }

    private async Task<SetView> ApplyCommands(SetView view, SetCommitCommand[] commitCommands)
    {
        foreach (var command in commitCommands)
        {
            view = command switch
            {
                NoOpCommand noOpCommand => ApplyCommand(view, noOpCommand),
                SetHomePlayersCommand setHomePlayersCommand => ApplyCommand(view, setHomePlayersCommand),
                SetAwayPlayersCommand setAwayPlayersCommand => ApplyCommand(view, setAwayPlayersCommand),
                UpdateSetScoreCommand updateSetScoreCommand => ApplyCommand(view, updateSetScoreCommand),
                SetCurrentServerCommand setCurrentServerCommand => ApplyCommand(view, setCurrentServerCommand),
                SetInitialServerCommand setInitialServerCommand => ApplyCommand(view, setInitialServerCommand),
                UpdateGameScoreCommand updateGameScoreCommand => ApplyCommand(view, updateGameScoreCommand),
                AddGameCommand addGameCommand => ApplyCommand(view, addGameCommand),
                AddWatchCommand addWatchCommand => ApplyCommand(view, addWatchCommand),
                UpdateWatchCommand updateWatchCommand => await ApplyCommand(view, updateWatchCommand),
                RemoveWatchCommand removeWatchCommand => ApplyCommand(view, removeWatchCommand),
                AddPenaltyEventCommand addPenaltyEventCommand => ApplyCommand(view, addPenaltyEventCommand),
                RemovePenaltyEventCommand removePenaltyEventCommand => ApplyCommand(view, removePenaltyEventCommand),
                _ => throw new Exception("Unable to apply command: " + command.GetType().FullName)
            };
        }

        return view;
    }

    private SetView ApplyCommand(SetView view, RemovePenaltyEventCommand command)
    {
        var penaltyEvents = view.PenaltyEvents.ToList();
        penaltyEvents.RemoveAll(x => x.PenaltyEventId == command.PenaltyEventId);
        return view with { PenaltyEvents = penaltyEvents.ToArray() };
    }

    private SetView ApplyCommand(SetView view, AddPenaltyEventCommand command)
    {
        if(view.PenaltyEvents.Any(x=>x.PenaltyEventId == command.PenaltyEvent.PenaltyEventId))
            throw new Exception("Penalty event already exists in set");
        
        var penaltyEvents = view.PenaltyEvents.ToList();
        penaltyEvents.Add(command.PenaltyEvent);
        return view with { PenaltyEvents = penaltyEvents.ToArray() };
    }

    private SetView ApplyCommand(SetView view, AddWatchCommand command)
    {
        // Check if watch already exists in set or game
        if (view.SetWatches.Any(x => x.Id == command.WatchId)) 
            throw new Exception("Watch already exists in set");
        if (view.Games.Any(y => y.Watches.Any(x => x.Id == command.WatchId)))
            throw new Exception("Watch already exists in game");

        if (command.GameIndex != null)
        {
            var games = view.Games.ToList();
            var watches = games[command.GameIndex.Value].Watches.ToList();
            var newWatch = new WatchView(command.WatchId, command.Key, Array.Empty<WatchChange>(),
                command.MaxMilliseconds);
            watches.Add(newWatch);
            games[command.GameIndex.Value] = games[command.GameIndex.Value] with { Watches = watches.ToArray() };
            return view with { Games = games.ToArray() };
        }
        else
        {
            var watches = view.SetWatches.ToList();
            var newWatch = new WatchView(command.WatchId, command.Key, Array.Empty<WatchChange>(),
                command.MaxMilliseconds);
            watches.Add(newWatch);
            return view with { SetWatches = watches.ToArray() };
        }
    }

    private async Task<SetView> ApplyCommand(SetView view, UpdateWatchCommand command)
    {
        if (!await _timestampService.IsValidTimestamp(command.Timestamp.ServerTimestamp))
        {
            throw new Exception("Invalid timestamp to update watch");
        }

        var watch = view.SetWatches.SingleOrDefault(x => x.Id == command.WatchId);
        List<WatchChange> changes;
        if (watch != null)
        {
            changes = watch.Changes.ToList();
            changes.Add(new WatchChange(command.NewState, command.Timestamp));
            watch = watch with { Changes = changes.ToArray() };
            return view with
            {
                SetWatches = view.SetWatches.Select(x => x.Id == command.WatchId ? watch : x).ToArray()
            };
        }

        for (var i = 0; i < view.Games.Length; i++)
        {
            var game = view.Games[i];
            watch = game.Watches.SingleOrDefault(x => x.Id == command.WatchId);

            if (watch == null) continue;

            changes = watch.Changes.ToList();
            changes.Add(new WatchChange(command.NewState, command.Timestamp));
            watch = watch with { Changes = changes.ToArray() };
            game = game with
            {
                Watches = game.Watches.Select(x => x.Id == command.WatchId ? watch : x).ToArray()
            };
            return view with
            {
                Games = view.Games.Select((x, y) => y == i ? game : x).ToArray()
            };
        }

        throw new Exception("Unable to update watch with id: " + command.WatchId);
    }

    private SetView ApplyCommand(SetView view, RemoveWatchCommand command)
    {
        if (view.SetWatches.Any(x => x.Id == command.WatchId))
            return view with { SetWatches = view.SetWatches.Where(x => x.Id != command.WatchId).ToArray() };
        if (view.Games.Any(x => x.Watches.Any(y => y.Id == command.WatchId)))
            return view with
            {
                Games = view.Games.Select(x =>
                    x with { Watches = x.Watches.Where(y => y.Id != command.WatchId).ToArray() }).ToArray()
            };

        throw new Exception("Unable to remove watch with id: " + command.WatchId);
    }

    private SetView ApplyCommand(SetView view, UpdateGameScoreCommand command)
    {
        var games = view.Games.ToList();
        var game = games[command.GameIndex];
        game = game with
        {
            Points = command.GameScore
        };
        games[command.GameIndex] = game;
        return view with { Games = games.ToArray() };
    }

    private SetView ApplyCommand(SetView view, SetInitialServerCommand command)
    {
        var games = view.Games.ToList();
        var game = games[command.GameIndex];
        game = game with
        {
            InitialServer = command.ServingPlayer,
            InitialReceiver = command.ReceivingPlayer
        };
        games[command.GameIndex] = game;
        return view with { Games = games.ToArray() };
    }

    private SetView ApplyCommand(SetView view, SetCurrentServerCommand command)
    {
        var games = view.Games.ToList();
        var game = games[command.GameIndex];
        game = game with
        {
            CurrentServer = command.ServingPlayer,
            CurrentReceiver = command.ReceivingPlayer
        };
        games[command.GameIndex] = game;
        return view with { Games = games.ToArray() };
    }

    private SetView ApplyCommand(SetView view, UpdateSetScoreCommand command)
    {
        return view with { GamesWon = command.SetScore };
    }

    private SetView ApplyCommand(SetView view, SetHomePlayersCommand command)
    {
        return view with { HomePlayers = command.HomePlayers };
    }

    private SetView ApplyCommand(SetView view, SetAwayPlayersCommand command)
    {
        return view with { AwayPlayers = command.AwayPlayers };
    }

    // ReSharper disable once UnusedParameter.Local
    private SetView ApplyCommand(SetView view, NoOpCommand command)
    {
        // Performs no changes.
        return view;
    }

    private SetView ApplyCommand(SetView view, AddGameCommand command)
    {
        var games = view.Games.ToList();
        var newGames = Enumerable.Range(0, command.Amount).Select(_ => new GameView
        {
            InitialServer = null,
            InitialReceiver = null,
            CurrentServer = null,
            CurrentReceiver = null,
            Points = Score.CreateZero(),
            Watches = Array.Empty<WatchView>()
        });
        games.InsertRange(command.Position, newGames);

        return view with { Games = games.ToArray() };
    }
}