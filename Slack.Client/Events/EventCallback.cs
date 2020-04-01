using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Events
{
    /// <summary>
    /// We package all event types delivered over the Events API in a common JSON-formatted event wrapper.
    /// The most common event wrapper is the event_callback.
    /// https://api.slack.com/types/event
    /// https://github.com/slackapi/slack-api-specs/blob/master/events-api/slack_common_event_wrapper_schema.json
    /// </summary>
    public class EventCallback : EventWrapper
    {
        public const string TypeName = "event_callback";

        public EventCallback()
        {
            Type = TypeName;
        }

        /// <summary>
        /// The unique identifier of the workspace where the event occurred.
        /// </summary>
        /// <example>T1H9RESGL</example>
        [Required]
        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }

        /// <summary>
        /// The unique identifier your installed Slack application.
        /// Use this to distinguish which app the event belongs to if you use multiple apps with the same Request URL.
        /// </summary>
        /// <example>A2H9RFS1A</example>
        [Required]
        [JsonPropertyName("api_app_id")]
        public string ApiAppId { get; set; }

        /// <summary>
        /// The actual event, an object, that happened. You'll find the most variance in properties beneath this node.
        /// </summary>
        [Required]
        [JsonPropertyName("event")]
        public Event Event { get; set; }

        /// <summary>
        /// A unique identifier for this specific event, globally unique across all workspaces.
        /// </summary>
        /// <example>Ev0PV52K25</example>
        [Required]
        [JsonPropertyName("event_id")]
        public string EventId { get; set; }

        /// <summary>
        /// The epoch timestamp in seconds indicating when this event was dispatched.
        /// </summary>
        /// <example>1525215129</example>
        [Required]
        [JsonPropertyName("event_time")]
        public int EventTime { get; set; }

        /// <summary>
        /// An array of string-based User IDs.
        /// Each member of the collection represents a user that has installed your application/bot,
        /// and indicates the described event would be visible to those users.
        /// </summary>
        /// <example>1525215129</example>
        [JsonPropertyName("authed_users")]
        public string[] AuthedUsers { get; set; }
    }
}
