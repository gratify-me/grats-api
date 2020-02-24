using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements
{
    /// <summary>
    /// A plain-text input, similar to the HTML <input> tag, creates a field where a user can enter freeform data.
    /// It can appear as a single-line field or a larger textarea using the multiline flag.
    /// Supported in the following app surfaces: Modals
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public class PlainTextInput : BlockElement
    {
        [JsonPropertyName("type")]
        public string Type => "plain_text_input";

        /// <summary>
        /// A text object that defines the placeholder text shown in the plain-text input.
        /// Maximum length for the text in this field is 150 characters.
        /// </summary>
        [JsonPropertyName("placeholder")]
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// The initial value in the plain-text input when it is loaded.
        /// </summary>
        [JsonPropertyName("initial_value")]
        public PlainText InitialValue { get; set; }

        /// <summary>
        /// Indicates whether the input will be a single line (false) or a larger textarea (true). Defaults to false.
        /// </summary>
        [JsonPropertyName("multiline")]
        public bool? Multiline { get; set; }

        /// <summary>
        /// The minimum length of input that the user must provide.If the user provides less, they will receive an error. Maximum value is 3000.
        /// </summary>
        [JsonPropertyName("min_length")]
        public int? MinLength { get; set; }

        /// <summary>
        /// The maximum length of input that the user can provide. If the user provides more, they will receive an error.
        /// </summary>
        [JsonPropertyName("max_length")]
        public int? MaxLength { get; set; }
    }
}
