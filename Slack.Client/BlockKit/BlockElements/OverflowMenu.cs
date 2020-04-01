using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements
{
    /// <summary>
    /// This is like a cross between a button and a select menu.
    /// When a user clicks on this overflow button, they will be presented with a list of options to choose from.
    /// Unlike the select menu, there is no typeahead field, and the button always appears with an ellipsis ("…") rather than customizable text.
    /// As such, it is usually used if you want a more compact layout than a select menu,
    /// or to supply a list of less visually important actions after a row of buttons.
    /// You can also specify simple URL links as overflow menu options, instead of actions.
    /// Works with block types: Section, Actions
    /// </summary>
    public class OverflowMenu : BlockElement
    {
        public const string TypeName = "overflow";

        public OverflowMenu()
        {
            Type = TypeName;
        }

        /// <summary>
        /// An array of option objects to display in the menu. Maximum number of options is 5, minimum is 2.
        /// </summary>
        [Required]
        [JsonPropertyName("options")]
        public Option[] Options { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after a menu item is selected.
        /// </summary>
        [JsonPropertyName("confirm")]
        public ConfirmationDialog Confirm { get; set; }
    }
}
