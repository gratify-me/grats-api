using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Events
{
    /// <summary>
    /// The actual event, an object, that happened. You'll find the most variance in properties beneath this node.
    /// https://api.slack.com/types/event
    /// https://github.com/slackapi/slack-api-specs/blob/master/events-api/slack_common_event_wrapper_schema.json
    /// </summary>
    public class Event
    {
        /// <summary>
        /// The specific name of the event.
        /// </summary>
        /// <example>app_home</example>
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// When the event was dispatched.
        /// </summary>
        /// <example>1525215129.000001</example>
        [JsonPropertyName("event_ts")]
        public string EventTs { get; set; }
    }
}
