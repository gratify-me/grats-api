using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.BlockElements.Selects
{
    /// <summary>
    /// This select menu will populate its options with a list of public channels visible to the current user in the active workspace.
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public class ChannelsSelect : Select
    {
        [JsonPropertyName("type")]
        public string Type => "channels_select";

        /// <summary>
        /// The ID of any valid public channel to be pre-selected when the menu loads.
        /// </summary>
        [JsonPropertyName("initial_channel")]
        public string InitialChannel { get; set; }
    }
}
