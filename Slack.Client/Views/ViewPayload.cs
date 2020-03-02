using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Slack.Client.BlockKit.LayoutBlocks;

namespace Slack.Client.Views
{
    /// <summary>
    /// Views are app-customized visual areas within modals and Home tabs.
    /// To define these views, apps create view payloads — packages of information that describe layout, interactivity, and other useful metadata.
    /// https://api.slack.com/reference/surfaces/views
    /// </summary>
    public abstract class ViewPayload
    {
        /// <summary>
        /// An array of blocks that defines the content of the view. Max of 100 blocks.
        /// </summary>
        [Required]
        [JsonPropertyName("blocks")]
        public LayoutBlock[] Blocks { get; set; }

        /// <summary>
        /// An optional string that will be sent to your app in view_submission and block_actions events.
        /// Max length of 3000 characters.
        /// </summary>
        [JsonPropertyName("private_metadata")]
        public string PrivateMetadata { get; set; }

        /// <summary>
        /// An identifier to recognize interactions and submissions of this particular view.
        /// Don't use this to store sensitive information (use private_metadata instead).
        /// Max length of 255 characters.
        /// </summary>
        [JsonPropertyName("callback_id")]
        public string CallbackId { get; set; }

        /// <summary>
        /// A custom identifier that must be unique for all views on a per-team basis.
        /// </summary>
        [JsonPropertyName("external_id")]
        public bool ExternalId { get; set; }
    }
}
