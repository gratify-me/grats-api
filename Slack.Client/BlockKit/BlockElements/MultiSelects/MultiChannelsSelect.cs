using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.BlockElements.MultiSelects
{
    /// <summary>
    /// This multi-select menu will populate its options with a list of public channels visible to the current user in the active workspace.
    /// Works with block types: Section, Input
    /// </summary>
    public class MultiChannelsSelect : MultiSelect
    {
        public const string TypeName = "multi_channels_select";

        public MultiChannelsSelect()
        {
            Type = TypeName;
        }

        /// <summary>
        /// An array of one or more IDs of any valid public channel to be pre-selected when the menu loads.
        /// </summary>
        [JsonPropertyName("initial_channels")]
        public string[] InitialChannels { get; set; }
    }
}
