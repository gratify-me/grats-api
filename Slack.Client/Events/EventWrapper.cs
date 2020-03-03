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
        /// A verification token to validate the event originated from Slack.
        /// </summary>
        /// <example>Jhj5dZrVaK7ZwHHjRyZWjbDl</example>
        [Required]
        [JsonPropertyName("token")]
        public string Token { get; set; }

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
        public object Event { get; set; }

        /// <summary>
        /// Indicates which kind of event dispatch this is, usually `event_callback`.
        /// </summary>
        /// <example>event_callback</example>
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }

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
        [Required]
        [JsonPropertyName("authed_users")]
        public string[] AuthedUsers { get; set; }
    }
}
