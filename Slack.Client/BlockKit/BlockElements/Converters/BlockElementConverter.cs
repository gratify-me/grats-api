using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.BlockElements.MultiSelects;
using Slack.Client.BlockKit.BlockElements.Selects;

namespace Slack.Client.BlockKit.BlockElements.Converters
{
    public class BlockElementConverter : JsonConverter<BlockElement>
    {
        private readonly JsonSerializerOptions _options;

        public BlockElementConverter()
        {
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            };
        }

        public override bool CanConvert(Type typeToConvert) => typeof(BlockElement).IsAssignableFrom(typeToConvert);

        public override BlockElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                Button.TypeName => JsonSerializer.Deserialize<Button>(jsonObject, _options),
                CheckboxGroup.TypeName => JsonSerializer.Deserialize<CheckboxGroup>(jsonObject, _options),
                DatePicker.TypeName => JsonSerializer.Deserialize<DatePicker>(jsonObject, _options),
                ImageElement.TypeName => JsonSerializer.Deserialize<ImageElement>(jsonObject, _options),
                OverflowMenu.TypeName => JsonSerializer.Deserialize<OverflowMenu>(jsonObject, _options),
                PlainTextInput.TypeName => JsonSerializer.Deserialize<PlainTextInput>(jsonObject, _options),
                RadioButtonGroup.TypeName => JsonSerializer.Deserialize<RadioButtonGroup>(jsonObject, _options),
                // Select converters
                ChannelsSelect.TypeName => JsonSerializer.Deserialize<ChannelsSelect>(jsonObject, _options),
                ConversationsSelect.TypeName => JsonSerializer.Deserialize<ConversationsSelect>(jsonObject, _options),
                ExternalSelect.TypeName => JsonSerializer.Deserialize<ExternalSelect>(jsonObject, _options),
                StaticSelect.TypeName => JsonSerializer.Deserialize<StaticSelect>(jsonObject, _options),
                UsersSelect.TypeName => JsonSerializer.Deserialize<UsersSelect>(jsonObject, _options),
                // MultiSelect converters
                MultiChannelsSelect.TypeName => JsonSerializer.Deserialize<MultiChannelsSelect>(jsonObject, _options),
                MultiConversationsSelect.TypeName => JsonSerializer.Deserialize<MultiConversationsSelect>(jsonObject, _options),
                MultiExternalSelect.TypeName => JsonSerializer.Deserialize<MultiExternalSelect>(jsonObject, _options),
                MultiStaticSelect.TypeName => JsonSerializer.Deserialize<MultiStaticSelect>(jsonObject, _options),
                MultiUsersSelect.TypeName => JsonSerializer.Deserialize<MultiUsersSelect>(jsonObject, _options),
                _ => throw new JsonException(),
            };
        }

        public override void Write(Utf8JsonWriter writer, BlockElement slackEvent, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)slackEvent, _options);
        }
    }
}
