using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements
{
    /// <summary>
    /// A checkbox group that allows a user to choose multiple items from a list of possible options.
    /// Checkboxes are only supported in the following app surfaces: Home tabs Modals
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public class CheckboxGroup : BlockElement
    {
        public const string TypeName = "checkboxes";

        public CheckboxGroup()
        {
            Type = TypeName;
        }

        public CheckboxGroup(string id, Option[] options)
        {
            Type = TypeName;
            ActionId = id;
            Options = options;
        }

        public CheckboxGroup(string id, Option[] options, Option initialOption)
        {
            Type = TypeName;
            ActionId = id;
            Options = options;
            InitialOptions = new Option[] { initialOption };
        }

        /// <summary>
        /// An array of option objects.
        /// </summary>
        [Required]
        [JsonPropertyName("options")]
        public Option[] Options { get; set; }

        /// <summary>
        /// An array of option objects that exactly matches one or more of the options within options.
        /// These options will be selected when the checkbox group initially loads.
        /// </summary>
        [JsonPropertyName("initial_options")]
        public Option[] InitialOptions { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after clicking one of the checkboxes in this element.
        /// </summary>
        [JsonPropertyName("confirm")]
        public ConfirmationDialog Confirm { get; set; }

        /// <summary>
        /// The options selected by the user, as returned from Slack.
        /// </summary>
        [JsonPropertyName("selected_options")]
        public Option[] SelectedOptions { get; set; }
    }
}
