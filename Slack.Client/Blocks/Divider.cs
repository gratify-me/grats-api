using System.Text.Json.Serialization;

namespace Slack.Client.Blocks
{
    /// <summary>
    /// A content divider, like an <hr>, to split up different blocks inside of a message.
    /// The divider block is nice and neat, requiring only a type.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Divider
    {
        [JsonPropertyName("type")]
        public string Type => "divider";

        /// <summary>
        /// A string acting as a unique identifier for a block.
        /// You can use this block_id when you receive an interaction payload to identify the source of the action.
        /// If not specified, one will be generated. Maximum length for this field is 255 characters.
        /// block_id should be unique for each message and each iteration of a message.
        /// If a message is updated, use a new block_id.
        /// </summary>
        [JsonPropertyName("block_id")]
        public string BlockId { get; set; }
    }
}
