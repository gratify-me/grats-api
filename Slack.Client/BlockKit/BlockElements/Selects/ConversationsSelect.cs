using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.BlockElements.Selects
{
    /// <summary>
    /// This select menu will populate its options with a list of public and private channels, DMs, and MPIMs
    /// visible to the current user in the active workspace.
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public class ConversationsSelect : Select
    {
        public const string TypeName = "conversations_select";

        public ConversationsSelect()
        {
            Type = TypeName;
        }

        /// <summary>
        /// An array of one or more IDs of any valid conversations to be pre-selected when the menu loads.
        /// </summary>
        [JsonPropertyName("initial_conversation")]
        public string InitialConversation { get; set; }
    }
}
