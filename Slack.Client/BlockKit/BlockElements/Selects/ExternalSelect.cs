using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;

namespace Slack.Client.BlockKit.BlockElements.Selects
{
    /// <summary>
    /// This select menu will load its options from an external data source, allowing for a dynamic list of options.
    /// Works with block types: Section, Actions, Input
    /// </summary>
    public class ExternalSelect : Select
    {
        [JsonPropertyName("type")]
        public string Type => "external_select";

        /// <summary>
        /// A single option that exactly matches one of the options within the options or option_groups loaded from the external data source.
        /// This option will be selected when the menu initially loads.
        /// </summary>
        [JsonPropertyName("initial_option")]
        public Option InitialOption { get; set; }

        /// <summary>
        /// When the typeahead field is used, a request will be sent on every character change.
        /// If you prefer fewer requests or more fully ideated queries,
        /// use the min_query_length attribute to tell Slack the fewest number of typed characters required before dispatch.
        /// </summary>
        [JsonPropertyName("min_query_length")]
        public int MinQueryLength { get; set; }
    }
}
