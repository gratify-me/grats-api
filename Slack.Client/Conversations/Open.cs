using System.Text.Json.Serialization;

namespace Slack.Client.Conversations
{
    /// <summary>
    /// Arguments used by conversations.open https://api.slack.com/methods/conversations.open
    /// </summary>
    public class Open
    {
        /// <summary>
        /// Resume a conversation by supplying an im or mpim's ID. Or provide the users field instead.
        /// </summary>
        /// <example>G1234567890</example>
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Boolean, indicates you want the full IM channel definition in the response.
        /// </summary>
        /// <example>true</example>
        [JsonPropertyName("return_im")]
        public bool? ReturnIm { get; set; }

        /// <summary>
        /// Comma separated lists of users. If only one user is included, this creates a 1:1 DM.
        /// The ordering of the users is preserved whenever a multi-person direct message is returned.
        /// Supply a channel when not supplying users.
        /// </summary>
        /// <example>W1234567890,U2345678901,U3456789012</example>
        [JsonPropertyName("users")]
        public string Users { get; set; }
    }
}
