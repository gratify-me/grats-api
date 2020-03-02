using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.LayoutBlocks;

namespace Slack.Client.Chat
{
    /// <summary>
    /// All of the Slack APIs that publish messages use a common base structure, called a message payload.
    /// This is a JSON object that is used to define metadata about the message, such as where it should be published, as well as its visual composition.
    /// https://api.slack.com/reference/messaging/payload
    /// </summary>
    public class MessagePayload
    {
        /// <summary>
        /// The usage of this field changes depending on whether you're using blocks or not.
        /// If you are, this is used as a fallback string to display in notifications.
        /// If you aren't, this is the main body text of the message. It can be formatted as plain text, or with mrkdwn.
        /// This field is not enforced as required when using blocks, however it is highly recommended that you include it as the aforementioned fallback.
        /// </summary>
        /// <example>Hello world!</example>
        [Required]
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// An array of layout blocks in the same format as described in the building blocks guide: https://api.slack.com/block-kit/building
        /// </summary>
        [JsonPropertyName("blocks")]
        public LayoutBlock[] Blocks { get; set; }

        /// <summary>
        /// The ID of another un-threaded message to reply to: https://api.slack.com/messaging/managing#threading
        /// </summary>
        /// <example>1234567890.123456</example>
        [JsonPropertyName("thread_ts")]
        public string ThreadTs { get; set; }

        /// <summary>
        /// Determines whether the text field is rendered according to mrkdwn formatting or not. Defaults to true.
        /// </summary>
        [JsonPropertyName("mrkdwn")]
        public bool? Mrkdwn { get; set; }
    }
}
