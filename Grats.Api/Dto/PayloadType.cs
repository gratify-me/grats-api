using Newtonsoft.Json;

namespace Gratify.Grats.Api.Dto
{
    public class PayloadType
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        public bool IsViewSubmission =>
            Type != null
            && Type == "view_submission";
    }
}
