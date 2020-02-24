using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.CompositionObjects
{
    /// <summary>
    /// Provides a way to group options in a select menu or multi-select menu.
    /// </summary>
    public class OptionGroup : CompositionObject
    {
        /// <summary>
        /// A text object that defines the label shown above this group of options.
        /// Maximum length for the text in this field is 75 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("label")]
        public PlainText Label { get; set; }

        /// <summary>
        /// An array of option objects that belong to this specific group. Maximum of 100 items.
        /// </summary>
        [Required]
        [JsonPropertyName("options")]
        public Option[] Options { get; set; }
    }
}
