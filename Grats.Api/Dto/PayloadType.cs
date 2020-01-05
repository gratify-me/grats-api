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

        public bool IsViewClosed =>
            Type != null
            && Type == "view_closed";

        // https://api.slack.com/reference/interaction-payloads/block-actions
        public bool IsBlockActions =>
            Type != null
            && Type == "block_actions";

        // https://api.slack.com/reference/interaction-payloads/actions
        public bool IsMessageActions =>
            Type != null
            && Type == "message_actions";
    }
}
