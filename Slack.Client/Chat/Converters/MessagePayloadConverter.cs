using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.LayoutBlocks.Converters;

namespace Slack.Client.Chat.Converters
{
    public class MessagePayloadConverter : JsonConverter<MessagePayload>
    {
        private readonly JsonSerializerOptions _options;

        public MessagePayloadConverter()
        {
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };
            _options.Converters.Add(new LayoutBlockConverter());
        }

        public override bool CanConvert(Type typeToConvert) => typeof(MessagePayload).IsAssignableFrom(typeToConvert);

        public override MessagePayload Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException("Deserialization of MessagePayload and its subclasses is not supported.");
        }

        public override void Write(Utf8JsonWriter writer, MessagePayload slackEvent, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)slackEvent, _options);
        }
    }
}
