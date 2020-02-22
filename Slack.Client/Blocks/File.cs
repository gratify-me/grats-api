using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Blocks
{
    /// <summary>
    /// Displays a remote file.
    /// Available in surfaces: Messages
    /// </summary>
    public class File : LayoutBlock
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
    }
}
