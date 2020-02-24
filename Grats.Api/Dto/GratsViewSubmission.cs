using System.Text.Json.Serialization;

namespace Gratify.Grats.Api.Dto
{
    // https://api.slack.com/reference/interaction-payloads
    public class GratsViewSubmission
    {
        // "type":"view_submission"
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

        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }

        [JsonPropertyName("view")]
        public View View { get; set; }
    }

    public class View
    {
        [JsonPropertyName("state")]
        public State State { get; set; }

        [JsonPropertyName("callback_id")]
        public string CallbackId { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }

    public class State
    {
        [JsonPropertyName("values")]
        public StateValues Values { get; set; }
    }

    public class StateValues
    {
        [JsonPropertyName("select_user")]
        public SelectUser SelectUser { get; set; }

        [JsonPropertyName("grats_message")]
        public GratsMessage GratsMessage { get; set; }
    }

    public class SelectUser
    {
        [JsonPropertyName("user_selected")]
        public UsersSelect UsersSelect { get; set; }
    }

    public class UsersSelect
    {
        [JsonPropertyName("selected_user")]
        public string SelectedUser { get; set; }
    }

    public class GratsMessage
    {
        [JsonPropertyName("grats_message_written")]
        public PlainTextInput PlainTextInput { get; set; }
    }

    public class PlainTextInput
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
