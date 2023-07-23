using System.Collections;
using T3.Web.Services.Commit.Models;

namespace T3.Web.Services.Commit;

public static class SetCommitBodyTypes
{
    public static IReadOnlyDictionary<string, Type> TypeMap => _typeMap.AsReadOnly();

    private static readonly Dictionary<string, Type> _typeMap = new()
    {
        {nameof(SetCommitBodyType.NoOp), typeof(NoOpCommand)},
        {nameof(SetCommitBodyType.SetHomeTeam), typeof(SetHomeTeamCommand)},
        {nameof(SetCommitBodyType.SetAwayTeam), typeof(SetAwayTeamCommand)},
        {nameof(SetCommitBodyType.SetInitialServer), typeof(SetInitialServerCommand)},
        {nameof(SetCommitBodyType.SetCurrentServer), typeof(SetCurrentServerCommand)},
        {nameof(SetCommitBodyType.UpdateSetScore), typeof(UpdateSetScoreCommand)},
        {nameof(SetCommitBodyType.UpdateGameScore), typeof(UpdateGameScoreCommand)},
        {nameof(SetCommitBodyType.AddGame), typeof(AddGameCommand)},
        {nameof(SetCommitBodyType.AddWatch), typeof(AddWatchCommand)},
        {nameof(SetCommitBodyType.UpdateWatch), typeof(UpdateWatchCommand)},
        {nameof(SetCommitBodyType.RemoveWatch), typeof(RemoveWatchCommand)},
        {nameof(SetCommitBodyType.AddPenaltyEvent), typeof(AddPenaltyEventCommand)},
        {nameof(SetCommitBodyType.UpdatePenaltyEvent), typeof(UpdatePenaltyEventCommand)},
        {nameof(SetCommitBodyType.RemovePenaltyEvent), typeof(RemovePenaltyEventCommand)},
    };

    static SetCommitBodyTypes()
    {
        // Self test if all types are unique
        var uniqueTypes = TypeMap.Values.Distinct().Count();
        if (uniqueTypes != TypeMap.Count)
        {
            throw new Exception("Not all types are mapped to a unique type");
        }
    }

    public static IEnumerable<Type> GetTypes() => TypeMap.Values;
}