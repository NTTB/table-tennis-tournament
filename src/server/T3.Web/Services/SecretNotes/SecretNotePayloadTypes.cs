using T3.Web.Services.SecretNotes.Entities;

namespace T3.Web.Services.SecretNotes;

public static class SecretNotePayloadTypes
{
    public static IReadOnlyDictionary<string, Type> TypeMap => _typeMap.AsReadOnly();

    private static readonly Dictionary<string, Type> _typeMap = new()
    {
        {nameof(SecretNotePayloadType.Plain), typeof(SecretNoteContentPlain)},
    };

    static SecretNotePayloadTypes()
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