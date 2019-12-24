using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gratify.Grats.Api.Dto
{
    // https://api.slack.com/reference/interaction-payloads
    public class GratsViewSubmission
    {
        // "type":"view_submission"
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("team")]
        public Team Team { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("api_app_id")]
        public string ApiAppId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        [JsonProperty("view")]
        public View View { get; set; }
    }

    public class View
    {
        [JsonProperty("state")]
        public State Type { get; set; }
    }

    public class State
    {
        [JsonProperty("values")]
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> Values { get; set; }
    }
}
