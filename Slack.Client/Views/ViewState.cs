using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Slack.Client.Views
{
    /// <summary>
    /// User-supplied values, sent as a part of a ViewSubmission interaction.
    /// </summary>
    public class ViewState
    {
        /// <summary>
        /// A dictionary of objects keyed with the block_ids of any user-modified input blocks from the modal view.
        /// Within each block_id object is another object keyed by the action_id of the child block element of the input block.
        /// This final child object will contain the type and submitted value of the input block element.
        /// </summary>
        [JsonPropertyName("values")]
        public Dictionary<string, JsonElement> Values { get; set; }
    }
}
