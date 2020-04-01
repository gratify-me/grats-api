using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Slack.Client.Events.Converters
{
    public class EventWrapperConverter : JsonConverter<EventWrapper>
    {
        private readonly JsonSerializerOptions _options;

        public EventWrapperConverter()
        {
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            };
            _options.Converters.Add(new EventConverter());
        }

        public override bool CanConvert(Type typeToConvert) => typeof(EventWrapper).IsAssignableFrom(typeToConvert);

        public override EventWrapper Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                EventCallback.TypeName => JsonSerializer.Deserialize<EventCallback>(jsonObject, _options),
                UrlVerificationRequest.TypeName => JsonSerializer.Deserialize<UrlVerificationRequest>(jsonObject, _options),
                _ => throw new JsonException(),
            };
        }

        public override void Write(Utf8JsonWriter writer, EventWrapper slackEvent, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)slackEvent, _options);
        }
    }
}
