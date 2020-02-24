using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements
{
    /// <summary>
    /// An element which lets users easily select a date from a calendar style UI.
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public class DatePicker : BlockElement
    {
        [JsonPropertyName("type")]
        public string Type => "datepicker";

        /// <summary>
        /// A text object that defines the placeholder text shown on the datepicker.
        /// Maximum length for the text in this field is 150 characters.
        /// </summary>
        [JsonPropertyName("placeholder")]
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// The initial date that is selected when the element is loaded. This should be in the format YYYY-MM-DD.
        /// </summary>
        [JsonPropertyName("initial_date")]
        public string InitialDate { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after a date is selected.
        /// </summary>
        [JsonPropertyName("confirm")]
        public ConfirmationDialog Confirm { get; set; }
    }
}
