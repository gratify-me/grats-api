using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.CompositionObjects
{
    /// <summary>An object containing some text formatted as plain_text</summary>
    public class PlainText : TextObject
    {
        [JsonPropertyName("type")]
        public string Type => "plain_text";

        /// <summary>Indicates whether emojis in a text field should be escaped into the colon emoji format.</summary>
        [JsonPropertyName("emoji")]
        public bool? Emoji { get; set; }
    }
}
