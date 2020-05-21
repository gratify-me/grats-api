using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.LayoutBlocks;

namespace Slack.Client.Chat
{
    /// <summary>
    /// Arguments used by chat.update https://api.slack.com/methods/chat.update
    /// </summary>
    public class UpdateMessage : MessagePayload
    {
        public UpdateMessage()
        { }

        public UpdateMessage(string text, LayoutBlock[] blocks, ApiResponse originalMessage)
        {
            Text = text;
            Blocks = blocks;
            Channel = originalMessage.Channel;
            Timestamp = originalMessage.Timestamp;
        }

        /// <summary>
        /// Channel containing the message to be updated.
        /// </summary>
        /// <example>C1234567890</example>
        [Required]
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Timestamp of the message to be updated.
        /// </summary>
        /// <example>1405894322.002768</example>
        [Required]
        [JsonPropertyName("ts")]
        public string Timestamp { get; set; }

        /// <summary>
        /// Pass true to update the message as the authed user. Bot users in this context are considered authed users.
        /// </summary>
        /// <example>true</example>
        [JsonPropertyName("as_user")]
        public bool? AsUser { get; set; }

        /// <summary>
        /// A JSON-based array of structured attachments, presented as a URL-encoded string. This field is required when not presenting text. If you don't include this field, the message's previous attachments will be retained. To remove previous attachments, include an empty array for this field.
        /// </summary>
        /// <example>[{"pretext": "pre-hello", "text": "text-world"}]</example>
        [JsonPropertyName("attachments")]
        public bool? Attachments { get; set; }

        /// <summary>
        /// Find and link channel names and usernames. Defaults to none. If you do not specify a value for this field, the original value set for the message will be overwritten with the default, none.
        /// </summary>
        /// <example>true</example>
        [JsonPropertyName("link_names")]
        public bool? LinkNames { get; set; }

        /// <summary>
        /// Change how messages are treated. Defaults to client, unlike chat.postMessage. Accepts either none or full. If you do not specify a value for this field, the original value set for the message will be overwritten with the default, client.
        /// </summary>
        /// <example>none</example>
        [JsonPropertyName("parse")]
        public string Parse { get; set; }
    }
}
