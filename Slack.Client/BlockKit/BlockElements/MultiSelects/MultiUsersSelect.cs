using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.BlockElements.MultiSelects
{
    /// <summary>
    /// This multi-select menu will populate its options with a list of Slack users visible to the current user in the active workspace.
    /// Works with block types: Section, Input
    /// </summary>
    public class MultiUsersSelect : MultiSelect
    {
        [JsonPropertyName("type")]
        public string Type => "multi_users_select";

        /// <summary>
        /// An array of user IDs of any valid users to be pre-selected when the menu loads.
        /// </summary>
        [JsonPropertyName("initial_users")]
        public string[] InitialUsers { get; set; }
    }
}
