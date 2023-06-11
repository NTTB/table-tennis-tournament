using System.Text.Json;
using System.Text.Json.Serialization;
using T3.Web.Services.SecretNotes.Entities;

namespace T3.Web.Services.SecretNotes;

public class SecretNotePayloadConvertor : JsonConverter<SecretNoteContent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == (typeof(SecretNoteContent));
    }

    public override SecretNoteContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        const string typenameDefaultCase = nameof(SecretNoteContent.Type);
        var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
        if (!json.TryGetProperty(typenameDefaultCase.ToLowerInvariant(), out var typeElement))
        {
            if (!json.TryGetProperty(typenameDefaultCase, out typeElement))
            {
                throw new Exception("Type property is missing");
            }
        }

        var type = typeElement.GetString();
        if (type == null) throw new Exception("TypeElement doesn't return a string");

        if (SecretNotePayloadTypes.TypeMap.ContainsKey(type) == false)
            throw new NotImplementedException("No converter for type: " + type);

        var targetType = SecretNotePayloadTypes.TypeMap[type];
        return JsonSerializer.Deserialize(json.GetRawText(), targetType, options) as SecretNoteContent;
    }

    public override void Write(Utf8JsonWriter writer, SecretNoteContent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}