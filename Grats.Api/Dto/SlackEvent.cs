using Newtonsoft.Json;

namespace Gratify.Grats.Api.Dto
{
    // https://api.slack.com/events/app_home_opened
    public class SlackEvent
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("team_id")]
        public string TeamId { get; set; }

        [JsonProperty("api_app_id")]
        public string ApiAppId { get; set; }

        [JsonProperty("event")]
        public EventCallback Event { get; set; }

        [JsonProperty("challenge")]
        public string Challenge { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("event_id")]
        public string EventId { get; set; }

        [JsonProperty("event_time")]
        public long EventTime { get; set; }

        public bool IsUrlVerification => Type == "url_verification";
    }

    public class EventCallback
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("tab")]
        public string Tab { get; set; }

        public bool IsAppHomeOpened => Type == "app_home_opened";
    }
}
