using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Blocks
{
    /// <summary>
    /// Displays message context, which can include both images and text.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Context
    {
        [JsonPropertyName("type")]
        public string Type => "context";

        /// <summary>
        /// An array of image elements and text objects. Maximum number of items is 10.
        /// </summary>
        [Required]
        [JsonPropertyName("elements")]
        public object[] Elements { get; set; }

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
