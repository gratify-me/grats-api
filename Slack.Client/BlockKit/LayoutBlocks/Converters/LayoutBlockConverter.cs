using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.BlockElements.Converters;
using Slack.Client.BlockKit.CompositionObjects.Converters;

namespace Slack.Client.BlockKit.LayoutBlocks.Converters
{
    public class LayoutBlockConverter : JsonConverter<LayoutBlock>
    {
        private readonly JsonSerializerOptions _options;

        public LayoutBlockConverter()
        {
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            };
            _options.Converters.Add(new TextObjectConverter());
            _options.Converters.Add(new BlockElementConverter());
        }

        public override bool CanConvert(Type typeToConvert) => typeof(LayoutBlock).IsAssignableFrom(typeToConvert);

        public override LayoutBlock Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                Actions.TypeName => JsonSerializer.Deserialize<Actions>(jsonObject, _options),
                Context.TypeName => JsonSerializer.Deserialize<Context>(jsonObject, _options),
                Divider.TypeName => JsonSerializer.Deserialize<Divider>(jsonObject, _options),
                File.TypeName => JsonSerializer.Deserialize<File>(jsonObject, _options),
                ImageBlock.TypeName => JsonSerializer.Deserialize<ImageBlock>(jsonObject, _options),
                Input.TypeName => JsonSerializer.Deserialize<Input>(jsonObject, _options),
                Section.TypeName => JsonSerializer.Deserialize<Section>(jsonObject, _options),
                _ => throw new JsonException(),
            };
        }

        public override void Write(Utf8JsonWriter writer, LayoutBlock slackEvent, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)slackEvent, _options);
        }
    }
}
