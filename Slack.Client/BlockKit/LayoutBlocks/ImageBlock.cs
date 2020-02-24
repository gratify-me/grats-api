using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.LayoutBlocks
{
    /// <summary>
    /// A simple image block, designed to make those cat photos really pop.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class ImageBlock : LayoutBlock
    {
        [JsonPropertyName("type")]
        public string Type => "image";

        /// <summary>
        /// The URL of the image to be displayed. Maximum length for this field is 3000 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("image_url")]
        public Uri ImageUrl { get; set; }

        /// <summary>
        /// A plain-text summary of the image.This should not contain any markup. Maximum length for this field is 2000 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("alt_text")]
        public string AltText { get; set; }

        /// <summary>
        /// An optional title for the image in the form of a text object. Maximum length for the text in this field is 2000 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("title")]
        public PlainText Title { get; set; }
    }
}
