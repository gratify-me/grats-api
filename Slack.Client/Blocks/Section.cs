using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Blocks
{
    /// <summary>
    /// A section is one of the most flexible blocks available
    /// it can be used as a simple text block, in combination with text fields, or side-by-side with any of the available block elements.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Section
    {
        [JsonPropertyName("type")]
        public string Type => "section";

        /// <summary>
        /// The text for the block, in the form of a text object.
        /// Maximum length for the text in this field is 3000 characters.
        /// This field is not required if a valid array of fields objects is provided instead.
        /// </summary>
        [Required]
        [JsonPropertyName("text")]
        public TextObject Text { get; set; }

        /// <summary>
        /// A string acting as a unique identifier for a block.
        /// You can use this block_id when you receive an interaction payload to identify the source of the action.
        /// If not specified, one will be generated. Maximum length for this field is 255 characters.
        /// block_id should be unique for each message and each iteration of a message.
        /// If a message is updated, use a new block_id.
        /// </summary>
        [JsonPropertyName("block_id")]
        public string BlockId { get; set; }

        /// <summary>
        /// An array of text objects.
        /// Any text objects included with fields will be rendered in a compact format that allows for 2 columns of side-by-side text.
        /// Maximum number of items is 10. Maximum length for the text in each item is 2000 characters.
        /// </summary>
        [JsonPropertyName("fields")]
        public TextObject[] Fields { get; set; }

        /// <summary>
        /// One of the available element objects:
        /// Button, Checkboxes, Date Picker, Image, Multi-select Menu, Overflow Menu, Plain-text input, Radio button group or Select Menus.
        /// </summary>
        [JsonPropertyName("accessory")]
        public object Accessory { get; set; }
    }
}
