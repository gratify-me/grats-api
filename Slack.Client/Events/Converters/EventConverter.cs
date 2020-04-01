using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Slack.Client.Events.Converters
{
    public class EventConverter : JsonConverter<Event>
    {
        private readonly JsonSerializerOptions _options;

        public EventConverter()
        {
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };
        }

        public override bool CanConvert(Type typeToConvert) => typeof(Event).IsAssignableFrom(typeToConvert);

        public override Event Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                AppHomeOpened.TypeName => JsonSerializer.Deserialize<AppHomeOpened>(jsonObject, _options),
                _ => throw new JsonException(),
            };
        }

        public override void Write(Utf8JsonWriter writer, Event slackEvent, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)slackEvent, _options);
        }
    }
}
