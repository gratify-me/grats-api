using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.LayoutBlocks
{
    /// <summary>
    /// A section is one of the most flexible blocks available
    /// it can be used as a simple text block, in combination with text fields, or side-by-side with any of the available block elements.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Section : LayoutBlock
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
        public object Text { get; set; } // TODO: Should be TextObject but: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to#serialize-properties-of-derived-classes

        /// <summary>
        /// An array of text objects.
        /// Any text objects included with fields will be rendered in a compact format that allows for 2 columns of side-by-side text.
        /// Maximum number of items is 10. Maximum length for the text in each item is 2000 characters.
        /// </summary>
        [JsonPropertyName("fields")]
        public object[] Fields { get; set; } // TODO: Should be TextObject but: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to#serialize-properties-of-derived-classes

        /// <summary>
        /// One of the available element objects:
        /// Button, Checkboxes, Date Picker, Image, Multi-select Menu, Overflow Menu, Plain-text input, Radio button group or Select Menus.
        /// </summary>
        [JsonPropertyName("accessory")]
        public object Accessory { get; set; }
    }
}
