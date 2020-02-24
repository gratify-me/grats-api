using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements
{
    /// <summary>
    /// A radio button group that allows a user to choose one item from a list of possible options.
    /// Supported in the following app surfaces: Home tabs, Modals
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public abstract class RadioButtonGroup : BlockElement
    {
        [JsonPropertyName("type")]
        public string Type => "radio_buttons";

        /// <summary>
        /// An array of option objects.
        /// </summary>
        [Required]
        [JsonPropertyName("options")]
        public Option[] Options { get; set; }

        /// <summary>
        /// An option object that exactly matches one of the options within options.
        /// This option will be selected when the radio button group initially loads.
        /// </summary>
        [JsonPropertyName("initial_option")]
        public Option InitialOption { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after clicking one of the radio buttons in this element.
        /// </summary>
        [JsonPropertyName("confirm")]
        public ConfirmationDialog Confirm { get; set; }
    }
}
