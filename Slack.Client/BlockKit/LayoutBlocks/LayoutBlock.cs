using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.LayoutBlocks
{
    /// <summary>
    /// Blocks are a series of components that can be combined to create visually rich and compellingly interactive messages. https://api.slack.com/reference/block-kit/blocks
    /// </summary>
    public class LayoutBlock
    {
        /// <summary>
        /// A string that indicates which kind of layout block this is.
        /// </summary>
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// A string acting as a unique identifier for a block.
        /// You can use this block_id when you receive an interaction payload to identify the source of the action.
        /// If not specified, one will be generated. Maximum length for this field is 255 characters.
        /// block_id should be unique for each message and each iteration of a message.
        /// If a message is updated, use a new block_id.
        /// </summary>
        [Required]
        [JsonPropertyName("block_id")]
        public string BlockId { get; set; }
    }
}
