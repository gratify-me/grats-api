using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.LayoutBlocks
{
    /// <summary>
    /// Displays message context, which can include both images and text.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Context : LayoutBlock
    {
        [JsonPropertyName("type")]
        public string Type => "context";

        /// <summary>
        /// An array of image elements and text objects. Maximum number of items is 10.
        /// </summary>
        [Required]
        [JsonPropertyName("elements")]
        public object[] Elements { get; set; }
    }
}
