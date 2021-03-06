﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Views
{
    /// <summary>
    /// If your app just received a view_submission payload, you have 3 seconds to respond, and update the source view.
    /// Respond to the HTTP request app with a subtype of ResponseAction, along with a newly composed view.
    /// https://api.slack.com/surfaces/modals/using
    /// </summary>
    public class ResponseActionUpdate : ResponseAction
    {
        [JsonPropertyName("response_action")]
        public string ResponseActionType => "update";

        /// <summary>
        /// A view payload.
        /// </summary>
        [Required]
        [JsonPropertyName("view")]
        public ViewPayload View { get; set; }
    }
}
