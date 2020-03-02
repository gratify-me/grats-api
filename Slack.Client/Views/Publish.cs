using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Views
{
    /// <summary>Arguments used by views.publish https://api.slack.com/methods/views.publish</summary>
    public class Publish
    {
        /// <summary>
        /// Id of the user you want publish a view to.
        /// </summary>
        /// <example>U0BPQUNTA</example>
        [Required]
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// A view payload.
        /// </summary>
        [Required]
        [JsonPropertyName("view")]
        public ViewPayload View { get; set; }

        /// <summary>
        /// A string that represents view state to protect against possible race conditions.
        /// </summary>
        /// <example>156772938.1827394</example>
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
