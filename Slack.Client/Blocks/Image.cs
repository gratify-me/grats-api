using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Blocks
{
    /// <summary>
    /// A simple image block, designed to make those cat photos really pop.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Image
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
