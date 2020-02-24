using System.Text.Json.Serialization;

namespace Gratify.Grats.Api.Dto
{
    // https://api.slack.com/reference/interaction-payloads
    public class InteractionPayload
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("team")]
        public Team Team { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("api_app_id")]
        public string ApiAppId { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("container")]
        public Container Container { get; set; }

        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }

        [JsonPropertyName("channel")]
        public Channel Channel { get; set; }

        [JsonPropertyName("response_url")]
        public string ResponseUrl { get; set; }

        [JsonPropertyName("actions")]
        public Action[] Actions { get; set; }
    }

    public class Team
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }
    }

    public class Container
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("message_ts")]
        public string MessageTs { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [JsonPropertyName("is_ephemeral")]
        public bool IsEphemeral { get; set; }
    }

    public class Channel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Action
    {
        [JsonPropertyName("action_id")]
        public string ActionId { get; set; }

        [JsonPropertyName("block_id")]
        public string BlockId { get; set; }

        [JsonPropertyName("text")]
        public Text Text { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("style")]
        public string Style { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("action_ts")]
        public string ActionTs { get; set; }
    }

    public class Text
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Value { get; set; }

        [JsonPropertyName("emoji")]
        public bool Emoji { get; set; }
    }
}
