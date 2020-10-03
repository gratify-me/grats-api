using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.LayoutBlocks
{
    /// <summary>
    /// A header is a plain-text block that displays in a larger, bold font.
    /// Use it to delineate between different groups of content in your app's surfaces.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Header : LayoutBlock
    {
        public Header()
        {
            Type = TypeName;
        }

        public Header(string text)
        {
            Type = TypeName;
            Text = new PlainText(text);
        }

        public const string TypeName = "header";

        /// <summary>
        /// The text for the block, in the form of a plain_text text object.
        /// Maximum length for the text in this field is 3000 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("text")]
        public PlainText Text { get; set; }
    }
}
