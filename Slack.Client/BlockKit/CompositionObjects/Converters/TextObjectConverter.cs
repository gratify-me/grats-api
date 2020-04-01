using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.CompositionObjects.Converters
{
    public class TextObjectConverter : JsonConverter<TextObject>
    {
        private readonly JsonSerializerOptions _options;

        public TextObjectConverter()
        {
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            };
        }

        public override bool CanConvert(Type typeToConvert) => typeof(TextObject).IsAssignableFrom(typeToConvert);

        public override TextObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDocument = JsonDocument.ParseValue(ref reader);
            if (!jsonDocument.RootElement.TryGetProperty("type", out var typeProperty))
            {
                throw new JsonException();
            }

            var jsonObject = jsonDocument.RootElement.GetRawText();
            return typeProperty.GetString() switch
            {
                PlainText.TypeName => JsonSerializer.Deserialize<PlainText>(jsonObject, _options),
                MrkdwnText.TypeName => JsonSerializer.Deserialize<MrkdwnText>(jsonObject, _options),
                _ => throw new JsonException(),
            };
        }

        public override void Write(Utf8JsonWriter writer, TextObject textObject, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)textObject, _options);
        }
    }
}
