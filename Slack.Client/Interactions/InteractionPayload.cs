using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.Primitives;

namespace Slack.Client.Interactions
{
    /// <summary>
    /// Interaction payloads are sent to your app when an end-user interacts with one of a range of Slack app features.
    /// The resulting payload can have different structures depending on the source.All those structures will have a type field that indicates the source of the interaction.
    /// https://api.slack.com/reference/interaction-payloads
    /// </summary>
    public abstract class InteractionPayload
    {
        /// <summary>
        /// Helps identify the source of the payload.
        /// Four types of interactions are rumored to exist: view_submission, view_closed, block_actions, and message_actions.
        /// </summary>
        /// <example>view_submission</example>
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The workspace that the interaction happened in.
        /// </summary>
        [JsonPropertyName("team")]
        public Team Team { get; set; }

        /// <summary>
        /// The user who interacted to trigger this request.
        /// </summary>
        [JsonPropertyName("user")]
        public User User { get; set; }

        /// <summary>
        /// The unique identifier your installed Slack application.
        /// Use this to distinguish which app the event belongs to if you use multiple apps with the same Request URL.
        /// </summary>
        /// <example>A2H9RFS1A</example>
        [Required]
        [JsonPropertyName("api_app_id")]
        public string ApiAppId { get; set; }

        /// <summary>
        /// A short-lived ID that can be used to open modals.
        /// </summary>
        [Required]
        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }

        /// <summary>
        /// A unique value which is optionally accepted in views.update and views.publish API calls.
        /// When provided to those APIs, the hash is validated such that only the most recent view can be updated. This should be used to ensure the correct view is being updated when updates are happening asynchronously.
        /// </summary>
        /// <example>156772938.1827394</example>
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
