using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Chat
{
    /// <summary>
    /// API response from chat.postMessage and chat.update
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Is true if request to the Slack API was successful, false otherwise.
        /// </summary>
        /// <example>true</example>
        [Required]
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        // In addition to channel and ts, the responce contains the message that was sent.
        // The message property is currently not mapped.
        /// <summary>
        /// Channel the message was sent to. Only available when Ok is true.
        /// </summary>
        /// <example>C1234567890</example>
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Timestamp of when the message was received. Only available when Ok is true.
        /// </summary>
        /// <example>1503435956.000247</example>
        [JsonPropertyName("ts")]
        public string Timestamp { get; set; }

        /// <summary>
        /// Error code describing why the message wasn't received successfully. Only available when Ok is false.
        /// </summary>
        /// <example>1503435956.000247</example>
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}
