using System.Text.Json.Serialization;
using Slack.Client.Primitives;

namespace Slack.Client.Conversations
{
    public class OpenResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("channel")]
        public Channel Channel { get; set; }
    }
}
