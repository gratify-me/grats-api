using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.CompositionObjects
{
    /// <summary>
    /// An object containing some text, formatted either as PlainText or MrkdwnText.
    /// </summary>
    public abstract class TextObject : CompositionObject
    {
        /// <summary>
        /// A text object that defines the button's text. Maximum length for the text in this field is 75 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("text")]
        public PlainText Text { get; set; }

        /// <summary>
        /// A URL to load in the user's browser when the button is clicked. Maximum length for this field is 3000 characters.
        /// If you're using url, you'll still receive an interaction payload and will need to send an acknowledgement response.
        /// </summary>
        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// The value to send along with the interaction payload. Maximum length for this field is 2000 characters.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// Decorates buttons with alternative visual color schemes. Use this option with restraint.
        /// "primary" gives buttons a green outline and text, ideal for affirmation or confirmation actions.
        /// primary should only be used for one button within a set.
        /// "danger" gives buttons a red outline and text, and should be used when the action is destructive.
        /// Use danger even more sparingly than primary.
        /// If you don't include this field, the default button style will be used.
        /// </summary>
        [JsonPropertyName("style")]
        public string Style { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog after the button is clicked.
        /// </summary>
        [JsonPropertyName("confirm")]
        public object Confirm { get; set; }
    }
}
