﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Blocks
{
    /// <summary>
    /// A block that collects information from users
    /// It can hold a plain-text input element, a select menu element, a multi-select menu element, or a datepicker.
    /// Available in surfaces: Modals
    /// </summary>
    public class Input : LayoutBlock
    {
        [JsonPropertyName("type")]
        public string Type => "input";

        /// <summary>
        /// A label that appears above an input element in the form of a text object.
        /// Maximum length for the text in this field is 2000 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("label")]
        public PlainText Label { get; set; }

        /// <summary>
        /// An plain-text input element, a select menu element, a multi-select menu element, or a datepicker.
        /// </summary>
        [Required]
        [JsonPropertyName("element")]
        public object[] Element { get; set; }

        /// <summary>
        /// An optional hint that appears below an input element in a lighter grey.
        /// Maximum length for the text in this field is 2000 characters.
        /// </summary>
        [JsonPropertyName("hint")]
        public PlainText Hint { get; set; }

        /// <summary>
        /// A boolean that indicates whether the input element may be empty when a user submits the modal. Defaults to false.
        /// </summary>
        [JsonPropertyName("optional")]
        public bool? Optional { get; set; }
    }
}
