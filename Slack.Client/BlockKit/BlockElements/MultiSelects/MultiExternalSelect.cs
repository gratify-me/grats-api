using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements.MultiSelects
{
    /// <summary>
    /// This menu will load its options from an external data source, allowing for a dynamic list of options.
    /// Works with block types: Section, Input
    /// </summary>
    public class MultiExternalSelect : MultiSelect
    {
        [JsonPropertyName("type")]
        public string Type => "multi_external_select";

        /// <summary>
        /// When the typeahead field is used, a request will be sent on every character change.
        /// If you prefer fewer requests or more fully ideated queries,
        /// use the min_query_length attribute to tell Slack the fewest number of typed characters required before dispatch.
        /// </summary>
        [JsonPropertyName("min_query_length")]
        public int MinQueryLength { get; set; }

        /// <summary>
        /// An array of option objects that exactly match one or more of the options within options or option_groups.
        /// These options will be selected when the menu initially loads.
        /// </summary>
        [JsonPropertyName("initial_options")]
        public Option[] InitialOptions { get; set; }
    }
}
