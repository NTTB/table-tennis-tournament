using System.Text.Json;
using System.Text.Json.Serialization;

namespace T3.Web.Services.Commit;

public class MatchCommitBodyConvertor : JsonConverter<MatchCommitCommand>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == (typeof(MatchCommitCommand));
    }

    public override MatchCommitCommand? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
        if (!json.TryGetProperty("type", out var typeElement))
        {
            if (!json.TryGetProperty("Type", out typeElement))
            {
                throw new Exception("Type property is missing");
            }
        }

        var type = typeElement.GetString();
        if (type == null) throw new Exception("TypeElement doesn't return a string");

        if (MatchCommitBodyTypes.TypeMap.ContainsKey(type) == false)
            throw new NotImplementedException("No converter for type: " + type);

        var targetType = MatchCommitBodyTypes.TypeMap[type];
        return JsonSerializer.Deserialize(json.GetRawText(), targetType, options) as MatchCommitCommand;
    }

    public override void Write(Utf8JsonWriter writer, MatchCommitCommand value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}