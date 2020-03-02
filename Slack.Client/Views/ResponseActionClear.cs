using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Views
{
    /// <summary>
    /// An app has the ability to close views within a modal.
    /// This can happen only in response to the user clicking a submit button in the modal.
    /// If all views in the modal should be closed, set the response_action to clear
    /// https://api.slack.com/surfaces/modals/using
    /// </summary>
    public class ResponseActionClear : ResponseAction
    {
        [JsonPropertyName("response_action")]
        public string ResponseActionType => "clear";
    }
}
