using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Views
{
    /// <summary>
    /// Upon receiving a view_submission event, your app may want to validate any inputs from the submitted view.
    /// If your app detects and validation errors, say an invalid email or an empty required field,
    /// the app can respond to the payload with a response_action of errors and an errors object providing error messages.
    /// https://api.slack.com/surfaces/modals/using
    /// </summary>
    public class ResponseActionErrors : ResponseAction
    {
        [JsonPropertyName("response_action")]
        public string ResponseActionType => "errors";

        /// <summary>
        /// Within the errors dictionary, you supply a key that is the BlockId of the erroneous input block, and a value - the plain text error message to be displayed to the user.
        /// Your app is responsible for setting and tracking BlockIds when composing views.
        /// </summary>
        [Required]
        [JsonPropertyName("errors")]
        public Dictionary<string, string> Errors { get; set; }
    }
}
