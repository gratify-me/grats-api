using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.CompositionObjects
{
    /// <summary>
    /// An object containing some text formatted as plain_text
    /// </summary>
    public class PlainText : TextObject
    {
        public const string TypeName = "plain_text";

        public PlainText()
        {
            Type = TypeName;
        }

        public PlainText(string text, bool emoji = true)
        {
            Type = TypeName;
            Text = text;
            Emoji = emoji;
        }

        /// <summary>
        /// Indicates whether emojis in a text field should be escaped into the colon emoji format.
        /// </summary>
        [JsonPropertyName("emoji")]
        public bool? Emoji { get; set; }
    }
}
