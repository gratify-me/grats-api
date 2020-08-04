using System;
using System.Text.Json.Serialization;

namespace Slack.Client.Primitives
{
    /// <summary>
    /// An object containing the default fields of a user's workspace profile.
    ///
    /// https://api.slack.com/types/user
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// The professional title specified by the user in their workspace profile.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The phone number specified by the user in their workspace profile.
        /// </summary>
        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// The skype username specified by the user in their workspace profile.
        /// </summary>
        [JsonPropertyName("skype")]
        public string Skype { get; set; }

        /// <summary>
        /// The real name that the user specified in their workspace profile.
        /// </summary>
        /// <example>Egon Spengler</example>
        [JsonPropertyName("real_name")]
        public string RealName { get; set; }

        /// <summary>
        /// The real_name field, but with any non-Latin characters filtered out.
        /// </summary>
        /// <example>Egon Spengler</example>
        [JsonPropertyName("real_name_normalized")]
        public string RealNameNormalized { get; set; }

        /// <summary>
        /// Indicates the display name that the user has chosen to identify themselves by in their workspace profile. Do not use this field as a unique identifier for a user, as it may change at any time. Instead, use id and team_id in concert.
        /// </summary>
        /// <example>spengler</example>
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        /// <summary>
        /// The display_name field, but with any non-Latin characters filtered out.
        /// </summary>
        /// <example>spengler</example>
        [JsonPropertyName("display_name_normalized")]
        public string DisplayNameNormalized { get; set; }

        /// <summary>
        /// The status set by the user.
        /// </summary>
        /// <example>Print is dead</example>
        [JsonPropertyName("status_text")]
        public string StatusText { get; set; }

        /// <summary>
        /// An accompanying emoji to the status.
        /// </summary>
        /// <example>books</example>
        [JsonPropertyName("status_emoji")]
        public string StatusEmoji { get; set; }

        /// <summary>
        /// A duration until the status expires.
        /// </summary>
        /// <example>1502138999</example>
        [JsonPropertyName("status_expiration")]
        public long StatusExpiration { get; set; }

        /// <summary>
        /// I have no idea what this property represents. Feel free to add any helpful tips you discover.
        /// </summary>
        /// <example>ge3b51ca72de</example>
        [JsonPropertyName("avatar_hash")]
        public string AvatarHash { get; set; }

        /// <summary>
        /// The first (given) name of the user.
        /// </summary>
        /// <example>Matthew</example>
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last (family) name of the user.
        /// </summary>
        /// <example>Johnston</example>
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// An email address supplied by the user.
        /// The users:read.email OAuth scope is required to access the email field.
        /// </summary>
        /// <example>spengler@ghostbusters.example.com</example>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// The user's profile picture as in the original size uploaded by the user.
        /// </summary>
        /// <example>https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg</example>
        [JsonPropertyName("image_original")]
        public Uri ImageOriginal { get; set; }

        /// <summary>
        /// The user's profile picture as a 24px by 24px, web-viewable image (GIFs, JPEGs, or PNGs).
        /// </summary>
        /// <example>https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg</example>
        [JsonPropertyName("image_24")]
        public Uri Image24 { get; set; }

        /// <summary>
        /// The user's profile picture as a 32px by 32px, web-viewable image (GIFs, JPEGs, or PNGs).
        /// </summary>
        /// <example>https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg</example>
        [JsonPropertyName("image_32")]
        public Uri Image32 { get; set; }

        /// <summary>
        /// The user's profile picture as a 48px by 48px, web-viewable image (GIFs, JPEGs, or PNGs).
        /// </summary>
        /// <example>https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg</example>
        [JsonPropertyName("image_48")]
        public Uri Image48 { get; set; }

        /// <summary>
        /// The user's profile picture as a 72px by 72px, web-viewable image (GIFs, JPEGs, or PNGs).
        /// </summary>
        /// <example>https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg</example>
        [JsonPropertyName("image_72")]
        public Uri Image72 { get; set; }

        /// <summary>
        /// The user's profile picture as a 192px by 192px, web-viewable image (GIFs, JPEGs, or PNGs).
        /// </summary>
        /// <example>https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg</example>
        [JsonPropertyName("image_192")]
        public Uri Image192 { get; set; }

        /// <summary>
        /// The user's profile picture as a 512px by 512px, web-viewable image (GIFs, JPEGs, or PNGs).
        /// </summary>
        /// <example>https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg</example>
        [JsonPropertyName("image_512")]
        public Uri Image512 { get; set; }

        /// <summary>
        /// Id of the team that the user is part of.
        /// </summary>
        /// <example>T012AB3C4</example>
        [JsonPropertyName("team")]
        public Uri Team { get; set; }
    }
}
