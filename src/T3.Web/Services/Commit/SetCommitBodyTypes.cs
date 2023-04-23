using System.Collections;
using T3.Web.Services.Commit.Models;

namespace T3.Web.Services.Commit;

public static class SetCommitBodyTypes
{
    public static IReadOnlyDictionary<string, Type> TypeMap => _typeMap.AsReadOnly();

    private static readonly Dictionary<string, Type> _typeMap = new()
    {
        {nameof(SetCommitBodyType.NoOp), typeof(NoOpCommand)},
        {nameof(SetCommitBodyType.SetHomePlayers), typeof(SetHomePlayersCommand)},
        {nameof(SetCommitBodyType.SetAwayPlayers), typeof(SetAwayPlayersCommand)},
        {nameof(SetCommitBodyType.SetInitialService), typeof(SetInitialServiceCommand)},
        {nameof(SetCommitBodyType.SetCurrentService), typeof(SetCurrentServiceCommand)},
        {nameof(SetCommitBodyType.UpdateSetScore), typeof(UpdateSetScoreCommand)},
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