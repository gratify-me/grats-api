namespace Slack.Client.Views
{
    /// <summary>
    /// An app has the ability to close views within a modal.
    /// This can happen only in response to the user clicking a submit button in the modal.
    /// If only the current modal should be closed, just send an empty 200 OK response.
    /// https://api.slack.com/surfaces/modals/using
    /// </summary>
    public class ResponseActionClose : ResponseAction
    {
    }
}
