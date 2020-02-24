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
        public string Text { get; set; }
    }
}
