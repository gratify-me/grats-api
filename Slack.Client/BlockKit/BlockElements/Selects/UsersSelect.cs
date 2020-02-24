using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.BlockElements.Selects
{
    /// <summary>
    /// This select menu will populate its options with a list of Slack users visible to the current user in the active workspace.
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public class UsersSelect : Select
    {
        [JsonPropertyName("type")]
        public string Type => "users_select";

        /// <summary>
        /// The user ID of any valid user to be pre-selected when the menu loads.
        /// </summary>
        [JsonPropertyName("initial_user")]
        public string InitialUser { get; set; }
    }
}
