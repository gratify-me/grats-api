using System.Text.Json.Serialization;

namespace Slack.Client.Views
{
    /// <summary>
    /// The Home tab is a persistent, yet dynamic interface for apps.
    /// Present each of your users with a unique Home tab just for them, always found in the exact same place.
    /// https://api.slack.com/surfaces/tabs
    /// </summary>
    public class HomeTab : ViewPayload
    {
        public const string TypeName = "home";

        public HomeTab()
        {
            Type = TypeName;
        }
    }
}
