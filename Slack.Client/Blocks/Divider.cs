﻿using System.Text.Json.Serialization;

namespace Slack.Client.Blocks
{
    /// <summary>
    /// A content divider, like an <hr>, to split up different blocks inside of a message.
    /// The divider block is nice and neat, requiring only a type.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Divider : LayoutBlock
    {
        [JsonPropertyName("type")]
        public string Type => "divider";
    }
}
