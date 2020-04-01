using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.BlockElements
{
    /// <summary>
    /// An element to insert an image as part of a larger block of content.
    /// If you want a block with only an image in it, you're looking for the ImageBlock.
    /// Works with block types: Section, Context
    /// </summary>
    public class ImageElement : BlockElement
    {
        public const string TypeName = "image";

        public ImageElement()
        {
            Type = TypeName;
        }

        /// <summary>
        /// The URL of the image to be displayed.
        /// </summary>
        [Required]
        [JsonPropertyName("image_url")]
        public Uri ImageUrl { get; set; }

        /// <summary>
        /// A plain-text summary of the image. This should not contain any markup.
        /// </summary>
        [Required]
        [JsonPropertyName("alt_text")]
        public string AltText { get; set; }
    }
}
