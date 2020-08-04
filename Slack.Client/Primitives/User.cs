using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Primitives
{
    /// <summary>
    /// A user object contains information about a Slack workspace user.
    ///
    /// The composition of user objects can vary greatly depending on the API being used,
    /// or the context of each Slack workspace.
    /// Data that has not been supplied may not be present at all, may be null, or may contain an empty string.
    /// Therefore, consider the following a non-exhaustive list of potential user object fields you might encounter.
    ///
    /// https://api.slack.com/types/user
    /// </summary>
    public class User
    {
        public static User Slackbot => new User
        {
            Id = "USLACKBOT",
        };

        /// <summary>
        /// Identifier for this workspace user.
        /// It is unique to the workspace containing the user.
        /// Use this field together with team_id as a unique key when storing related data,
        /// or when specifying the user in API requests.
        /// We recommend considering the format of the string to be an opaque value, and not to rely on a particular structure.
        /// </summary>
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Identifier for this workspace.
        /// </summary>
        [Required]
        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }

        /// <summary>
        /// This user has been deactivated when the value of this field is true.
        /// Otherwise the value is false, or the field may not appear at all.
        /// </summary>
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        /// <summary>
        /// Indicates whether the user is an Admin of the current workspace.
        /// </summary>
        [JsonPropertyName("is_admin")]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Indicates whether the user is an authorized user of the calling app.
        /// </summary>
        [JsonPropertyName("is_app_user")]
        public bool IsAppUser { get; set; }

        /// <summary>
        /// Indicates whether the user is actually a bot user. Bleep bloop.
        /// </summary>
        [JsonPropertyName("is_bot")]
        public bool IsBot { get; set; }

        /// <summary>
        /// Indicates whether the user signed up for the workspace directly (false or the field is absent),
        /// or they joined via an invite (true).
        /// </summary>
        [JsonPropertyName("is_invited_user")]
        public bool IsInvitedUser { get; set; }

        /// <summary>
        /// Indicates whether the user is an Owner of the current workspace.
        /// </summary>
        [JsonPropertyName("is_owner")]
        public bool IsOwner { get; set; }

        /// <summary>
        /// Indicates whether the user is the Primary Owner of the current workspace.
        /// </summary>
        [JsonPropertyName("is_primary_owner")]
        public bool IsPrimaryOwner { get; set; }

        /// <summary>
        /// Indicates whether or not the user is a guest user.
        /// Use in combination with the is_ultra_restricted field to check if the user is a single-channel guest user.
        /// </summary>
        [JsonPropertyName("is_restricted")]
        public bool IsRestricted { get; set; }

        /// <summary>
        /// If true, this user belongs to a different workspace than the one associated with your app's token,
        /// and isn't in any shared channels visible to your app.
        /// If false (or this field is not present), the user is either from the same workspace as associated with your app's token,
        /// or they are from a different workspace, but are in a shared channel that your app has access to.
        /// Read our shared channels docs for more detail.
        /// </summary>
        [JsonPropertyName("is_stranger")]
        public bool IsStranger { get; set; }

        /// <summary>
        /// Indicates whether or not the user is a single-channel guest.
        /// </summary>
        [JsonPropertyName("is_ultra_restricted")]
        public bool IsUltraRestricted { get; set; }

        /// <summary>
        /// Contains a IETF language code that represents this user's chosen display language for Slack clients.
        /// Useful for localizing your apps.
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// A unix timestamp indicating when the user object was last updated.
        /// </summary>
        [JsonPropertyName("updated")]
        public long Updated { get; set; }

        public override bool Equals(object obj) => obj is User other && Equals(other);

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(User first, User second) => first.Id == second.Id;

        public static bool operator !=(User first, User second) => first.Id != second.Id;

        public bool Equals(User other) => Id == other.Id;

        public override string ToString() => Id.ToString();
    }
}
