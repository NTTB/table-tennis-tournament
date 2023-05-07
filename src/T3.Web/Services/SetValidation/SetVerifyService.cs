using System.Text.Json;
using T3.Web.Services.Commit;
using T3.Web.Services.Commit.Models;
using T3.Web.Services.Commit.ValueObjects;
using T3.Web.Services.SetValidation.Models;

namespace T3.Web.Services.SetValidation;

public interface ISetCommitValidateService
{
    Task<SetCommitValidationResult> ValidateCommit(CommitId commitId);
}

public class SetCommitValidateService : ISetCommitValidateService
{
    private readonly ISetCommitService _setCommitService;

    public SetCommitValidateService(ISetCommitService setCommitService)
    {
        _setCommitService = setCommitService;
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


        var allInvalidViews = GetInvalidViews(commits).ToArray();
        return new SetCommitValidationResult(
            Valid: !allInvalidViews.Any(),
            InvalidViews: allInvalidViews
        );
    }

    private IEnumerable<IncorrectViews> GetInvalidViews(Stack<SetCommit> commits)
    {
        var calculatedView = new SetView
        {
            GamesWon = Score.CreateZero(),
            HomePlayers = Array.Empty<PlayerView>(),
            AwayPlayers = Array.Empty<PlayerView>(),
            Games = Array.Empty<GameView>()
        };
        
        foreach (var commit in commits)
        {
            calculatedView = ApplyCommands(calculatedView, commit.Commands);

            // Due to view having arrays, we need to serialize it to compare it.
            var calculatedViewJson = JsonSerializer.Serialize(calculatedView);
            var actualViewJson = JsonSerializer.Serialize(commit.View);
            
            if(calculatedViewJson == actualViewJson) continue;
            
            yield return new IncorrectViews(commit.View, calculatedView);
        }
    }

    private SetView ApplyCommands(SetView view, SetCommitCommand[] commitCommands)
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
                _ => throw new Exception("Unable to apply command: " + command.GetType().FullName)
            };
        }

        return view;
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

    private SetView ApplyCommand(SetView view, NoOpCommand command)
    {
        // Performs no changes.
        return view;
    }

    private SetView ApplyCommand(SetView view, AddGameCommand command)
    {
        var games = view.Games.ToList();
        var newGames = Enumerable.Range(0, command.Amount).Select(_ => new GameView());
        games.InsertRange(command.Position, newGames);

        return view with { Games = games.ToArray() };
    }
}