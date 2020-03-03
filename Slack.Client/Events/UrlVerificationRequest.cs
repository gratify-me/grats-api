using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Events
{
    /// <summary>
    /// The events sent to your Request URL may contain sensitive information associated with the workspaces having approved your Slack app.
    /// To ensure that events are being delivered to a server under your direct control, we must verify your ownership by issuing you a challenge request.
    /// After you've completed typing your URL, we'll dispatch a HTTP POST to your request URL ontaining an UrlVerification object.
    /// In addition we'll verify your SSL certificate.
    /// </summary>
    public class UrlVerificationRequest
    {
        /// <summary>
        /// This payload is similarly formatted to other event types you'll encounter in the Events API.
        /// To help you differentiate url verification requests form other event types, we inform you that this is of the url_verification variety.
        /// </summary>
        /// <example>url_verification</example>
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// This deprecated verification token is proof that the request is coming from Slack on behalf of your application.
        /// You'll find this value in the "App Credentials" section of your app's application management interface.
        /// Verifying this value is more important when working with real events after this verification sequence has been completed.
        /// When responding to real events, always use the more secure signing secret process to verify Slack requests' authenticity.
        /// </summary>
        /// <example>Jhj5dZrVaK7ZwHHjRyZWjbDl</example>
        [Required]
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// A randomly generated string produced by Slack. The point of this little game of cat and mouse,
        /// is that you're going to respond to this request with a response body containing this value.
        /// </summary>
        /// <example>3eZbrw1aBm2rZgRNFdxV2595E9CY3gmdALWMmHkvFXO7tYXAYM8P</example>
        [Required]
        [JsonPropertyName("challenge")]
        public string Challenge { get; set; }
    }
}
