using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Events
{
    /// <summary>
    /// Once you receive a UrlVerificationRequest, complete the sequence by responding with HTTP 200 and the challenge attribute value.
    /// </summary>
    public class UrlVerificationResponse
    {
        public UrlVerificationResponse()
        { }

        public UrlVerificationResponse(UrlVerificationRequest request)
        {
            Challenge = request.Challenge;
        }

        /// <summary>
        /// A randomly generated string produced by Slack. This should be the value supplied by the UrlVerificationRequest
        /// </summary>
        /// <example>3eZbrw1aBm2rZgRNFdxV2595E9CY3gmdALWMmHkvFXO7tYXAYM8P</example>
        [Required]
        [JsonPropertyName("challenge")]
        public string Challenge { get; set; }
    }
}
