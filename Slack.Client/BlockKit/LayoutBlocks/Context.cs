using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.LayoutBlocks
{
    /// <summary>
    /// Displays message context, which can include both images and text.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Context : LayoutBlock
    {
        public Context()
        {
            Type = TypeName;
        }

        public Context(MrkdwnText[] elements)
        {
            Type = TypeName;
            Elements = elements;
        }

        public const string TypeName = "context";

        /// <summary>
        /// An array of image elements and text objects. Maximum number of items is 10.
        /// </summary>
        [Required]
        [JsonPropertyName("elements")]
        public MrkdwnText[] Elements { get; set; } // TODO: Currently fixing this to MrkdwnText, since that's the only type we're using.
    }
}
