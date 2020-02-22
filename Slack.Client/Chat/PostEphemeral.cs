using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Chat
{
    /// <summary>Arguments used by chat.postEphemeral https://api.slack.com/methods/chat.postEphemeral</summary>
    public class PostEphemeral : MessagePayload
    {
        /// <summary>Channel, private group, or IM channel to send message to. Can be an encoded ID, or a name. See below for more details.</summary>
        /// <example>C1234567890</example>
        [Required]
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        /// <summary>ID of the user who will receive the ephemeral message.The user should be in the channel specified by the channel argument.</summary>
        /// <example>U0BPQUNTA</example>
        [Required]
        [JsonPropertyName("user")]
        public string User { get; set; }

        /// <summary>Pass true to post the message as the authed user, instead of as a bot. Defaults to false. See authorship below. This argument may not be used with newer bot tokens.</summary>
        /// <example>true</example>
        [JsonPropertyName("as_user")]
        public bool? AsUser { get; set; }

        /// <summary>Emoji to use as the icon for this message. Overrides icon_url. Must be used in conjunction with as_user set to false, otherwise ignored. See authorship below. This argument may not be used with newer bot tokens.</summary>
        /// <example>:chart_with_upwards_trend:</example>
        [JsonPropertyName("icon_emoji")]
        public string IconEmoji { get; set; }

        /// <summary>URL to an image to use as the icon for this message. Must be used in conjunction with as_user set to false, otherwise ignored. See authorship below. This argument may not be used with newer bot tokens.</summary>
        /// <example>http://lorempixel.com/48/48</example>
        [JsonPropertyName("icon_url")]
        public Uri IconUrl { get; set; }

        /// <summary>Find and link channel names and usernames.</summary>
        /// <example>true</example>
        [JsonPropertyName("link_names")]
        public bool? LinkNames { get; set; }

        /// <summary>Change how messages are treated. Defaults to none. See below.</summary>
        /// <example>full</example>
        [JsonPropertyName("parse")]
        public string Parse { get; set; }

        /// <summary>Set your bot's user name. Must be used in conjunction with as_user set to false, otherwise ignored. See authorship below.</summary>
        /// <example>My Bot</example>
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
