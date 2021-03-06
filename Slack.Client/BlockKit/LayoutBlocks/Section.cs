﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.BlockElements;
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
        public Section()
        {
            Type = TypeName;
        }

        public Section(string id, string text, BlockElement accessory = null)
        {
            Type = TypeName;
            BlockId = id;
            Text = new MrkdwnText(text);
            Accessory = accessory;
        }

        public const string TypeName = "section";

        /// <summary>
        /// The text for the block, in the form of a text object.
        /// Maximum length for the text in this field is 3000 characters.
        /// This field is not required if a valid array of fields objects is provided instead.
        /// </summary>
        [Required]
        [JsonPropertyName("text")]
        public TextObject Text { get; set; }

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
        public BlockElement Accessory { get; set; }
    }
}
