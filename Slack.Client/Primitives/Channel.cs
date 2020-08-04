using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Primitives
{
    /// <summary>
    /// A channel object contains information about a workspace channel.
    ///
    /// https://api.slack.com/types/channel
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// Identifier for this channel.
        /// </summary>
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name parameter indicates the name of the channel-like thing, without a leading hash sign.
        /// Don't get too attached to that name. It might change. Don't bother storing it even.
        /// When thinking about channel-like things, think about their IDs and their type and the team/workspace they belong to.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public override bool Equals(object obj) => obj is Channel other && Equals(other);

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(Channel first, Channel second) => first.Id == second.Id;

        public static bool operator !=(Channel first, Channel second) => first.Id != second.Id;

        public bool Equals(Channel other) => Id == other.Id;

        public override string ToString() => Id.ToString();
    }
}
