using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Blocks
{
    /// <summary>
    /// Displays a remote file.
    /// Available in surfaces: Messages
    /// </summary>
    public class File
    {
        [JsonPropertyName("type")]
        public string Type => "file";

        /// <summary>
        /// The external unique ID for this file.
        /// </summary>
        [Required]
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; }

        /// <summary>
        /// At the moment, source will always be remote for a remote file.
        /// </summary>
        [Required]
        [JsonPropertyName("source")]
        public string Source { get; set; }

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
