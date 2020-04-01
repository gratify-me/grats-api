using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.LayoutBlocks.Converters;

namespace Slack.Client.Views.Converters
{
    public class ViewPayloadConverter : JsonConverter<ViewPayload>
    {
        private readonly JsonSerializerOptions _options;

        public ViewPayloadConverter()
        {
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };
            _options.Converters.Add(new LayoutBlockConverter());
        }

        public override bool CanConvert(Type typeToConvert) => typeof(ViewPayload).IsAssignableFrom(typeToConvert);

        public override ViewPayload Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                HomeTab.TypeName => JsonSerializer.Deserialize<HomeTab>(jsonObject, _options),
                Modal.TypeName => JsonSerializer.Deserialize<Modal>(jsonObject, _options),
                _ => throw new JsonException(),
            };
        }

        public override void Write(Utf8JsonWriter writer, ViewPayload slackEvent, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)slackEvent, _options);
        }
    }
}
