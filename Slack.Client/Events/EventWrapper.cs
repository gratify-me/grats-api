using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Events
{
    /// <summary>
    /// We package all event types delivered over the Events API in a common JSON-formatted event wrapper.
    /// https://api.slack.com/types/event
    /// https://github.com/slackapi/slack-api-specs/blob/master/events-api/slack_common_event_wrapper_schema.json
    /// </summary>
    public class EventWrapper
    {
        /// <summary>
        /// A string that indicates which kind of event dispatch this is.
        /// </summary>
        /// <example>event_callback</example>
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
