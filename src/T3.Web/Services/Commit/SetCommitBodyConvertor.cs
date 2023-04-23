using System.Text.Json;
using System.Text.Json.Serialization;
using T3.Web.Services.Commit.Models;

namespace T3.Web.Services.Commit;

public class SetCommitBodyConvertor : JsonConverter<SetCommitBody>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == (typeof(SetCommitBody));
    }

    public override SetCommitBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        return type switch
        {
            nameof(SetCommitBodyType.NoOp) => JsonSerializer.Deserialize<SetCommitBodyNoOp>(json.GetRawText(), options),
            nameof(SetCommitBodyType.SetScoreChange) => JsonSerializer.Deserialize<SetCommitBodySetScoreChange>(json.GetRawText(), options),
            _ => throw new NotImplementedException("No converter for type: " + type)
        };
    }

    public override void Write(Utf8JsonWriter writer, SetCommitBody value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}