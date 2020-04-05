using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.Primitives;

namespace Slack.Client.BlockKit.BlockElements.Selects
{
    /// <summary>
    /// This select menu will populate its options with a list of Slack users visible to the current user in the active workspace.
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public class UsersSelect : Select
    {
        public const string TypeName = "users_select";

        public UsersSelect()
        {
            Type = TypeName;
        }

        public UsersSelect(string id, string placeholder, string initialUser = null)
        {
            Type = TypeName;
            ActionId = id;
            Placeholder = new PlainText(placeholder);

            if (initialUser != null)
            {
                InitialUser = initialUser;
            }
        }

        /// <summary>
        /// The user ID of any valid user to be pre-selected when the menu loads.
        /// </summary>
        [JsonPropertyName("initial_user")]
        public string InitialUser { get; set; }

        /// <summary>
        /// The user ID of the selected user, as returned from Slack.
        /// </summary>
        [JsonPropertyName("selected_user")]
        public string SelectedUserId { get; set; }

        /// <summary>
        /// The selected user, as returned from Slack.
        /// NB: Only the Id property will be set.
        /// </summary>
        [JsonIgnore]
        public User SelectedUser => new User { Id = SelectedUserId };
    }
}
