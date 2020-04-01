using System.Text.Json.Serialization;
using Slack.Client.Views;

namespace Slack.Client.Events
{
    /// <summary>
    /// This app event notifies your app when a user has entered the App Home.
    /// Your Slack app must have a bot user configured and installed to use this event.
    /// If the user opens a tab within the App Home, the event payload for this event will reference that in the tab field.
    /// If they opened a Home tab, a view field will also be included, containing the current state of that Home tab,
    /// including the list of blocks, and various pieces of metadata.
    ///
    /// Note: app_home_opened events are sent each time a user enters into the App Home space.
    /// Verify that this is the first interaction between a user and your app before triggering your onboarding flow.
    ///
    /// https://api.slack.com/events/app_home_opened
    /// </summary>
    public class AppHomeOpened : Event
    {
        public const string TypeName = "app_home_opened";

        public AppHomeOpened()
        {
            Type = TypeName;
        }

        /// <summary>
        /// The user ID belonging to the user that incited this action.
        /// </summary>
        /// <example>U061F7AUR</example>
        [JsonPropertyName("user")]
        public string User { get; set; }

        /// <summary>
        /// The channel ID belonging where this action originated.
        /// </summary>
        /// <example>D0LAN2Q65</example>
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// The tab where the action originated.
        /// </summary>
        /// <example>home</example>
        [JsonPropertyName("tab")]
        public string Tab { get; set; }

        /// <summary>
        /// A view payload.
        /// </summary>
        [JsonPropertyName("view")]
        public ViewPayload View { get; set; }
    }
}
