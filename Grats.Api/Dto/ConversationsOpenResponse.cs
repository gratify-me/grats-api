using Newtonsoft.Json;

namespace Gratify.Grats.Api.Dto
{
    public class ConversationsOpenResponse
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }
    }
}
