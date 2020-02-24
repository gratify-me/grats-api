using System.Text.Json.Serialization;

namespace Gratify.Grats.Api.Dto
{
    // https://api.slack.com/events/app_home_opened
    public class SlackEvent
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }

        [JsonPropertyName("api_app_id")]
        public string ApiAppId { get; set; }

        [JsonPropertyName("event")]
        public EventCallback Event { get; set; }

        [JsonPropertyName("challenge")]
        public string Challenge { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("event_id")]
        public string EventId { get; set; }

        [JsonPropertyName("event_time")]
        public long EventTime { get; set; }

        public bool IsUrlVerification => Type == "url_verification";
    }

    public class EventCallback
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("tab")]
        public string Tab { get; set; }

        public bool IsAppHomeOpened => Type == "app_home_opened";
    }
}
