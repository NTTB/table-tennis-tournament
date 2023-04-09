using System.Text.Json;
using System.Text.Json.Serialization;
using T3.Web.Services.Set.Models;

public class SetCommitBodyConvertor : JsonConverter<ISetCommitBody>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == (typeof(ISetCommitBody));
    }

    public override ISetCommitBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        if (type == "NoOp")
            return JsonSerializer.Deserialize<SetCommitBodyNoOp>(json.GetRawText(), options);
        
        throw new NotImplementedException("No converter for type: " + type);
    }

    public override void Write(Utf8JsonWriter writer, ISetCommitBody value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}