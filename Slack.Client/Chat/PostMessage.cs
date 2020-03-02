using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Chat
{
    /// <summary>
    /// Arguments used by chat.postMessage https://api.slack.com/methods/chat.postMessage
    /// </summary>
    public class PostMessage : MessagePayload
    {
        /// <summary>
        /// Channel, private group, or IM channel to send message to. Can be an encoded ID, or a name. See below for more details.
        /// </summary>
        /// <example>C1234567890</example>
        [Required]
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Pass true to post the message as the authed user, instead of as a bot. Defaults to false. See authorship below. This argument may not be used with newer bot tokens.
        /// </summary>
        /// <example>true</example>
        [JsonPropertyName("as_user")]
        public bool? AsUser { get; set; }

        /// <summary>
        /// Emoji to use as the icon for this message. Overrides icon_url.
        /// Must be used in conjunction with as_user set to false, otherwise ignored.
        /// This argument may not be used with newer bot tokens.
        /// </summary>
        /// <example>:chart_with_upwards_trend:</example>
        [JsonPropertyName("icon_emoji")]
        public string IconEmoji { get; set; }

        /// <summary>
        /// URL to an image to use as the icon for this message. Must be used in conjunction with as_user set to false, otherwise ignored.
        /// This argument may not be used with newer bot tokens.
        /// </summary>
        /// <example>http://lorempixel.com/48/48</example>
        [JsonPropertyName("icon_url")]
        public Uri IconUrl { get; set; }

        /// <summary>
        /// Find and link channel names and usernames.
        /// </summary>
        /// <example>true</example>
        [JsonPropertyName("link_names")]
        public bool? LinkNames { get; set; }

        /// <summary>
        /// Change how messages are treated. Defaults to none.
        /// </summary>
        /// <example>full</example>
        [JsonPropertyName("parse")]
        public string Parse { get; set; }

        /// <summary>
        /// Used in conjunction with thread_ts and indicates whether reply should be made visible to everyone in the channel or conversation.
        /// Defaults to false.
        /// </summary>
        /// <example>true</example>
        [JsonPropertyName("reply_broadcast")]
        public bool? ReplyBroadcast { get; set; }

        /// <summary>
        /// Pass true to enable unfurling of primarily text-based content.
        /// </summary>
        /// <example>true</example>
        [JsonPropertyName("unfurl_links")]
        public bool? UnfurlLinks { get; set; }

        /// <summary>
        /// Pass false to disable unfurling of media content.
        /// </summary>
        /// <example>false</example>
        [JsonPropertyName("unfurl_media")]
        public bool? UnfurlMedia { get; set; }

        /// <summary>
        /// Set your bot's user name. Must be used in conjunction with as_user set to false, otherwise ignored.
        /// </summary>
        /// <example>My Bot</example>
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
