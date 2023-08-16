namespace T3.Web.Services.Commit;

public static class MatchCommitBodyTypes
{
    private static readonly Dictionary<string, Type> TypeMapInternal = new()
    {
        { nameof(MatchCommitBodyType.NoOp), typeof(NoOpCommand) },
        { nameof(MatchCommitBodyType.SetHomeTeam), typeof(MatchHomeTeamCommand) },
        { nameof(MatchCommitBodyType.SetAwayTeam), typeof(MatchAwayTeamCommand) },
        { nameof(MatchCommitBodyType.SetInitialServer), typeof(MatchInitialServerCommand) },
        { nameof(MatchCommitBodyType.SetCurrentServer), typeof(MatchCurrentServerCommand) },
        { nameof(MatchCommitBodyType.UpdateSetScore), typeof(UpdateMatchScoreCommand) },
        { nameof(MatchCommitBodyType.UpdateGameScore), typeof(UpdateGameScoreCommand) },
        { nameof(MatchCommitBodyType.AddGame), typeof(AddGameCommand) },
        { nameof(MatchCommitBodyType.AddWatch), typeof(AddWatchCommand) },
        { nameof(MatchCommitBodyType.UpdateWatch), typeof(UpdateWatchCommand) },
        { nameof(MatchCommitBodyType.RemoveWatch), typeof(RemoveWatchCommand) },
        { nameof(MatchCommitBodyType.AddPenaltyEvent), typeof(AddPenaltyEventCommand) },
        { nameof(MatchCommitBodyType.UpdatePenaltyEvent), typeof(UpdatePenaltyEventCommand) },
        { nameof(MatchCommitBodyType.RemovePenaltyEvent), typeof(RemovePenaltyEventCommand) }
    };

    static MatchCommitBodyTypes()
    {
        // Self test if all types are unique
        var uniqueTypes = TypeMap.Values.Distinct().Count();
        if (uniqueTypes != TypeMap.Count) throw new Exception("Not all types are mapped to a unique type");
    }

    public static IReadOnlyDictionary<string, Type> TypeMap => TypeMapInternal.AsReadOnly();
}