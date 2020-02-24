using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements.MultiSelects
{
    /// <summary>
    /// A multi-select menu allows a user to select multiple items from a list of options.
    /// Just like regular select menus, multi-select menus also include type-ahead functionality,
    /// where a user can type a part or all of an option string to filter the list.
    /// </summary>
    public abstract class MultiSelect : BlockElement
    {
        /// <summary>
        /// A text object that defines the placeholder text shown on the menu.
        /// Maximum length for the text in this field is 150 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("placeholder")]
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears before the multi-select choices are submitted.
        /// </summary>
        [JsonPropertyName("confirm")]
        public ConfirmationDialog Confirm { get; set; }

        /// <summary>
        /// Specifies the maximum number of items that can be selected in the menu. Minimum number is 1.
        /// </summary>
        [JsonPropertyName("max_selected_items")]
        public int MaxSelectedItems { get; set; }
    }
}
