using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.BlockElements.MultiSelects
{
    /// <summary>
    /// This multi-select menu will populate its options with a list of public and private channels, DMs, and MPIMs
    /// visible to the current user in the active workspace.
    /// Works with block types: Section, Input
    /// </summary>
    public class MultiConversationsSelect : MultiSelect
    {
        [JsonPropertyName("type")]
        public string Type => "multi_conversations_select";

        /// <summary>
        /// An array of one or more IDs of any valid conversations to be pre-selected when the menu loads.
        /// </summary>
        [JsonPropertyName("initial_conversations")]
        public string[] InitialConversations { get; set; }
    }
}
