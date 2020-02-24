using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements.Selects
{
    /// <summary>
    /// This is the simplest form of select menu, with a static list of options passed in when defining the element.
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public class StaticSelect : Select
    {
        [JsonPropertyName("type")]
        public string Type => "static_select";

        /// <summary>
        /// An array of option objects. Maximum number of options is 100. If option_groups is specified, this field should not be.
        /// </summary>
        [JsonPropertyName("options")]
        public Option[] Options { get; set; }

        /// <summary>
        /// An array of option group objects. Maximum number of option groups is 100. If options is specified, this field should not be.
        /// </summary>
        [JsonPropertyName("option_groups")]
        public OptionGroup[] OptionGroups { get; set; }

        /// <summary>
        /// A single option that exactly matches one of the options within options or option_groups.
        /// This option will be selected when the menu initially loads.
        /// </summary>
        [JsonPropertyName("initial_option")]
        public Option[] InitialOption { get; set; }
    }
}
