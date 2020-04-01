using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;

namespace Slack.Client.Views
{
    /// <summary>
    /// Modals provide focused spaces ideal for requesting and collecting data from users, or temporarily displaying dynamic and interactive information.
    /// For users, modals are prominent and pervasive — taking center stage in Slack ahead of any other interface element.
    /// https://api.slack.com/surfaces/modals
    /// </summary>
    public class Modal : ViewPayload
    {
        public const string TypeName = "modal";

        public Modal()
        {
            Type = TypeName;
        }

        public Modal(Type id, string title, string submit, string close, LayoutBlock[] blocks)
        {
            Type = TypeName;
            CallbackId = id.ToString();
            Title = new PlainText(title);
            Submit = new PlainText(submit);
            Close = new PlainText(close);
            Blocks = blocks;
        }

        /// <summary>
        /// The title that appears in the top-left of the modal.
        /// Must be a plain_text text element with a max length of 24 characters.
        /// </summary>
        [Required]
        [JsonPropertyName("title")]
        public PlainText Title { get; set; }

        /// <summary>
        /// An optional PlainText element that defines the text displayed in the close button at the bottom-right of the view.
        /// Max length of 24 characters.
        /// </summary>
        [JsonPropertyName("close")]
        public PlainText Close { get; set; }

        /// <summary>
        /// An optional plain_text element that defines the text displayed in the submit button at the bottom-right of the view.
        /// Submit is required when an input block is within the blocks array.
        /// Max length of 24 characters.
        /// </summary>
        [JsonPropertyName("submit")]
        public PlainText Submit { get; set; }

        /// <summary>
        /// When set to true, clicking on the close button will clear all views in a modal and close it.
        /// Defaults to false.
        /// </summary>
        [JsonPropertyName("clear_on_close")]
        public bool ClearOnClose { get; set; }

        /// <summary>
        /// Indicates whether Slack will send your request URL a view_closed event when a user clicks the close button.
        /// Defaults to false.
        /// </summary>
        [JsonPropertyName("notify_on_close")]
        public bool NotifyOnClose { get; set; }

        /// <summary>
        /// Unique identifier of a modal that has been submitted in a ViewSubmission.
        /// </summary>
        /// <example>VNHU13V36</example>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// A string that represents view state to protect against possible race conditions.
        /// Set by Slack when responding with a ViewSubmission.
        /// </summary>
        /// <example>156772938.1827394</example>
        [JsonPropertyName("state")]
        public ViewState State { get; set; }

        /// <summary>
        /// A string that represents view state to protect against possible race conditions.
        /// Set by Slack when responding with a ViewSubmission.
        /// </summary>
        /// <example>156772938.1827394</example>
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
