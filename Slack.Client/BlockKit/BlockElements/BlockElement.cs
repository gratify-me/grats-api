using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.BlockElements
{
    /// <summary>
    /// Block elements can be used inside of section, context, and actions layout blocks. Inputs can only be used inside of input blocks. https://api.slack.com/reference/block-kit/block-elements
    /// </summary>
    public class BlockElement
    {
        /// <summary>
        /// A string that indicates which kind of block element this is.
        /// </summary>
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// An identifier for this action.
        /// You can use this when you receive an interaction payload to identify the source of the action.
        /// Should be unique among all other action_ids used elsewhere by your app.
        /// Maximum length for this field is 255 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("action_id")]
        public string ActionId { get; set; }
    }
}
