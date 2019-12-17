using Newtonsoft.Json;

namespace Gratify.Grats.Api.Dto
{
    // https://api.slack.com/reference/interaction-payloads
    public class InteractionPayload
    {
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

        [JsonProperty("container")]
        public Container Container { get; set; }

        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("response_url")]
        public string ResponseUrl { get; set; }

        [JsonProperty("actions")]
        public Action[] Actions { get; set; }
    }

    public class Team
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("team_id")]
        public string TeamId { get; set; }
    }

    public class Container
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message_ts")]
        public string MessageTs { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("is_ephemeral")]
        public bool IsEphemeral { get; set; }
    }

    public class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Action
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("block_id")]
        public string BlockId { get; set; }

        [JsonProperty("text")]
        public Text Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("style")]
        public string Style { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("action_ts")]
        public string ActionTs { get; set; }
    }

    public class Text
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Value { get; set; }

        [JsonProperty("emoji")]
        public bool Emoji { get; set; }
    }
}
