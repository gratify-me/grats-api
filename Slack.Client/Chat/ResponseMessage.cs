using System.Text.Json.Serialization;

namespace Slack.Client.Chat
{
    /// <summary>
    /// Depending on the source, the interaction payload your app receives may contain a response_url.
    /// This response_url is unique to each payload, and can be used to publish messages back to the place where the interaction happened.
    /// https://api.slack.com/interactivity/handling#message_responses
    /// </summary>
    public class ResponseMessage : MessagePayload
    {
        public ResponseMessage()
        { }

        public ResponseMessage(string text, bool ephemeral = true)
        {
            Text = text;

            if (ephemeral)
            {
                ResponseType = "ephemeral";
            }
        }

        /// <summary>
        /// If you want to publish an ephemeral message, include an attribute response_type with your message JSON, and set its value to 'ephemeral'
        /// </summary>
        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }

        /// <summary>
        /// If your app received an interaction payload after an interactive component was used inside of a message,
        /// you can use request_url to update that source message.
        /// Include an attribute replace_original and set it to true.
        /// </summary>
        [JsonPropertyName("replace_original")]
        public bool ReplaceOriginal { get; set; }
    }
}
