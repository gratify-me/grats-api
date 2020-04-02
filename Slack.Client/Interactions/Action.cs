using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Interactions
{
    /// <summary>
    /// actions Contains data from the specific interactive component that was used.
    /// App surfaces can contain blocks with multiple interactive components,
    /// and each of those components can have multiple values selected by users.
    /// </summary>
    public class Action
    {
        /// <summary>
        /// Identifies the interactive component itself.
        /// Some blocks can contain multiple interactive components,
        /// so the block_id alone may not be specific enough to identify the source component.
        /// </summary>
        [Required]
        [JsonPropertyName("action_id")]
        public string ActionId { get; set; }

        /// <summary>
        /// Identifies the block within a surface that contained the interactive component that was used.
        /// </summary>
        [Required]
        [JsonPropertyName("block_id")]
        public string BlockId { get; set; }

        /// <summary>
        /// Set by your app when you composed the blocks,
        /// this is the value that was specified in the interactive component when an interaction happened.
        /// For example, a select menu will have multiple possible values depending on what the user picks from the menu,
        /// and value will identify the chosen option.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        public Guid CorrelationId => Guid.Parse(Value);
    }
}
