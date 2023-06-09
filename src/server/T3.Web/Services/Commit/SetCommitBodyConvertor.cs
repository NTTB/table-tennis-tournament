using System.Text.Json;
using System.Text.Json.Serialization;
using T3.Web.Services.Commit.Models;

namespace T3.Web.Services.Commit;

public class SetCommitBodyConvertor : JsonConverter<SetCommitCommand>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == (typeof(SetCommitCommand));
    }

    public override SetCommitCommand? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
        JsonElement typeElement;
        if (!json.TryGetProperty("type", out typeElement))
        {
            if (!json.TryGetProperty("Type", out typeElement))
            {
                throw new Exception("Type property is missing");
            }
        }

        var type = typeElement.GetString();
        if (type == null) throw new Exception("TypeElement doesn't return a string");

        if (SetCommitBodyTypes.TypeMap.ContainsKey(type) == false)
            throw new NotImplementedException("No converter for type: " + type);

        var targetType = SetCommitBodyTypes.TypeMap[type];
        return JsonSerializer.Deserialize(json.GetRawText(), targetType, options) as SetCommitCommand;
    }

    public override void Write(Utf8JsonWriter writer, SetCommitCommand value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}