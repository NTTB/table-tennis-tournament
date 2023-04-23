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

        return type switch
        {
            nameof(SetCommitBodyType.NoOp) => JsonSerializer.Deserialize<NoOpCommand>(json.GetRawText(), options),
            nameof(SetCommitBodyType.SetHomePlayers) => JsonSerializer.Deserialize<SetHomePlayersCommand>(json.GetRawText(), options),
            nameof(SetCommitBodyType.SetAwayPlayers) => JsonSerializer.Deserialize<SetAwayPlayersCommand>(json.GetRawText(), options),
            nameof(SetCommitBodyType.SetInitialService) => JsonSerializer.Deserialize<SetInitialServiceCommand>(json.GetRawText(), options),
            nameof(SetCommitBodyType.SetCurrentService) => JsonSerializer.Deserialize<SetCurrentServiceCommand>(json.GetRawText(), options),
            nameof(SetCommitBodyType.SetScoreChange) => JsonSerializer.Deserialize<ChangeSetScoreCommand>(json.GetRawText(), options),
            _ => throw new NotImplementedException("No converter for type: " + type)
        };
    }

    public override void Write(Utf8JsonWriter writer, SetCommitCommand value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}