using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Views
{
    /// <summary>
    /// If your app just received a view_submission payload, you have 3 seconds to respond, and push a new view.
    /// Respond to the HTTP request app with a response_action field of value push, along with a newly composed view.
    /// https://api.slack.com/surfaces/modals/using
    /// </summary>
    public class ResponseActionPush : ResponseAction
    {
        public ResponseActionPush()
        { }

        public ResponseActionPush(ViewPayload view)
        {
            View = view;
        }

        [JsonPropertyName("response_action")]
        public string ResponseActionType => "push";

        /// <summary>
        /// A view payload.
        /// </summary>
        [Required]
        [JsonPropertyName("view")]
        public ViewPayload View { get; set; }
    }
}
