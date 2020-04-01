using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Primitives
{
    public class Channel
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; }

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
