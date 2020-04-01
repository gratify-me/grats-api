using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.LayoutBlocks.Converters;

namespace Slack.Client.Interactions.Converters
{
    public class InteractionPayloadConverter : JsonConverter<InteractionPayload>
    {
        private readonly JsonSerializerOptions _options;

        public InteractionPayloadConverter()
        {
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };
            _options.Converters.Add(new LayoutBlockConverter());
        }

        public override bool CanConvert(Type typeToConvert) => typeof(InteractionPayload).IsAssignableFrom(typeToConvert);

        public override InteractionPayload Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                ViewSubmission.TypeName => JsonSerializer.Deserialize<ViewSubmission>(jsonObject, _options),
                BlockActions.TypeName => JsonSerializer.Deserialize<BlockActions>(jsonObject, _options),
                _ => throw new JsonException(),
            };
        }

        public override void Write(Utf8JsonWriter writer, InteractionPayload slackEvent, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)slackEvent, _options);
        }
    }
}
