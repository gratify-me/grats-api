using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Views
{
    /// <summary>Arguments used by views.open https://api.slack.com/methods/views.open</summary>
    public class Open
    {
        /// <summary>Exchange a trigger to post to the user.</summary>
        /// <example>12345.98765.abcd2358fdea</example>
        [Required]
        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }

        /// <summary>A view payload. This must be a JSON-encoded string.</summary>
        [Required]
        [JsonPropertyName("view")]
        public string View { get; set; }
    }
}
