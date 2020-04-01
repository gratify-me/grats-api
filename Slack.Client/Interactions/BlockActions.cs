using System.Text.Json.Serialization;
using Slack.Client.Primitives;

namespace Slack.Client.Interactions
{
    /// <summary>
    /// Type: block_actions
    /// Received when a user interacts with a Block Kit interactive component.
    /// https://api.slack.com/reference/interaction-payloads/block-actions
    /// </summary>
    public class BlockActions : InteractionPayload
    {
        public const string TypeName = "block_actions";

        public BlockActions()
        {
            Type = TypeName;
        }

        /// <summary>
        /// The channel the source interaction happened in.
        /// </summary>
        [JsonPropertyName("channel")]
        public Channel Channel { get; set; }

        /// <summary>
        /// A short-lived webhook that can be used to send messages in response to interactions.
        /// </summary>
        [JsonPropertyName("response_url")]
        public string ResponseUrl { get; set; }

        /// <summary>
        /// actions Contains data from the specific interactive component that was used.
        /// App surfaces can contain blocks with multiple interactive components,
        /// and each of those components can have multiple values selected by users.
        /// </summary>
        [JsonPropertyName("actions")]
        public Action[] Actions { get; set; }
    }
}
