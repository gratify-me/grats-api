using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Blocks
{
    public abstract class TextObject
    {
        /// <summary>The text for the block. This field accepts any of the standard text formatting markup when type is mrkdwn.</summary>
        [Required]
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
