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
        public State State { get; set; }
    }

    public class State
    {
        [JsonProperty("values")]
        public StateValues Values { get; set; }
    }

    public class StateValues
    {
        [JsonProperty("select_user")]
        public SelectUser SelectUser { get; set; }

        [JsonProperty("grats_message")]
        public GratsMessage GratsMessage { get; set; }
    }

    public class SelectUser
    {
        [JsonProperty("user_selected")]
        public UsersSelect UsersSelect { get; set; }
    }

    public class UsersSelect
    {
        [JsonProperty("selected_user")]
        public string SelectedUser { get; set; }
    }

    public class GratsMessage
    {
        [JsonProperty("grats_message_written")]
        public PlainTextInput PlainTextInput { get; set; }
    }

    public class PlainTextInput
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
