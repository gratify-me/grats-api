using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements.Selects
{
    /// <summary>
    /// A select menu, just as with a standard HTML <select> tag, creates a drop down menu with a list of options for a user to choose.
    /// The select menu also includes type-ahead functionality, where a user can type a part or all of an option string to filter the list.
    /// </summary>
    public abstract class Select : BlockElement
    {
        /// <summary>
        /// A text object that defines the placeholder text shown on the menu.
        /// Maximum length for the text in this field is 150 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("placeholder")]
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after a menu item is selected.
        /// </summary>
        [JsonPropertyName("confirm")]
        public ConfirmationDialog Confirm { get; set; }
    }
}
