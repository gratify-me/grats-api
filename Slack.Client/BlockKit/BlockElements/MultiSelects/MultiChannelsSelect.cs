using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.BlockElements.MultiSelects
{
    /// <summary>
    /// This multi-select menu will populate its options with a list of public channels visible to the current user in the active workspace.
    /// Works with block types: Section, Input
    /// </summary>
    public class MultiChannelsSelect : MultiSelect
    {
        [JsonPropertyName("type")]
        public string Type => "multi_conversations_select";

        /// <summary>
        /// An array of one or more IDs of any valid public channel to be pre-selected when the menu loads.
        /// </summary>
        [JsonPropertyName("initial_channels")]
        public string[] InitialChannels { get; set; }
    }
}
