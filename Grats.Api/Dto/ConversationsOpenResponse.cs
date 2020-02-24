using System.Text.Json.Serialization;

namespace Gratify.Grats.Api.Dto
{
    public class ConversationsOpenResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("channel")]
        public Channel Channel { get; set; }
    }
}
