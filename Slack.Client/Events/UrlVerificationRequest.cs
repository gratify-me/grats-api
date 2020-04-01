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
    public class UrlVerificationRequest : EventWrapper
    {
        public const string TypeName = "url_verification";

        public UrlVerificationRequest()
        {
            Type = TypeName;
        }

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
