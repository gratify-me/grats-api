using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.CompositionObjects
{
    /// <summary>
    /// An object that represents a single selectable item in a select menu, multi-select menu, radio button group, or overflow menu.
    /// </summary>
    public class Option : CompositionObject
    {
        public static Option Yes => new Option("Yes");

        public static Option No => new Option("No");

        public Option()
        { }

        public Option(string value)
        {
            Value = value;
            Text = new PlainText(value, false);
        }

        /// <summary>
        /// A text object that defines the text shown in the option on the menu.
        /// Maximum length for the text in this field is 75 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("text")]
        public PlainText Text { get; set; }

        /// <summary>
        /// The string value that will be passed to your app when this option is chosen.
        /// Maximum length for this field is 75 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// A text object that defines a line of descriptive text shown below the text field beside the radio button.
        /// Maximum length for the text object within this field is 75 characters.
        /// </summary>
        [JsonPropertyName("description")]
        public PlainText Description { get; set; }

        /// <summary>
        /// A URL to load in the user's browser when the option is clicked.
        /// The url attribute is only available in overflow menus. Maximum length for this field is 3000 characters.
        /// If you're using url, you'll still receive an interaction payload and will need to send an acknowledgement response.
        /// </summary>
        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        public override bool Equals(object obj) => obj is Option other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Option first, Option second) => first.Value == second.Value;

        public static bool operator !=(Option first, Option second) => first.Value != second.Value;

        public bool Equals(Option other) => Value == other.Value;

        public override string ToString() => Value.ToString();
    }
}
