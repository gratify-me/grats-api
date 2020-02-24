using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.CompositionObjects
{
    /// <summary>
    /// An object that defines a dialog that provides a confirmation step to any interactive element.
    /// This dialog will ask the user to confirm their action by offering a confirm and deny buttons.
    /// </summary>
    public class ConfirmationDialog : CompositionObject
    {
        /// <summary>
        /// A text object that defines the dialog's title. Maximum length for this field is 100 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("title")]
        public PlainText Title { get; set; }

        /// <summary>
        /// A text object that defines the explanatory text that appears in the confirm dialog.
        /// Maximum length for the text in this field is 300 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("text")]
        public object Text { get; set; } // TODO: Should be TextObject but: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to#serialize-properties-of-derived-classes

        /// <summary>
        /// A text object to define the text of the button that confirms the action.
        /// Maximum length for the text in this field is 30 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("confirm")]
        public PlainText Confirm { get; set; }

        /// <summary>
        /// A text object to define the text of the button that cancels the action.
        /// Maximum length for the text in this field is 30 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("deny")]
        public PlainText Deny { get; set; }
    }
}
