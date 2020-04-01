using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Primitives
{
    public class Team
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        public override bool Equals(object obj) => obj is Team other && Equals(other);

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(Team first, Team second) => first.Id == second.Id;

        public static bool operator !=(Team first, Team second) => first.Id != second.Id;

        public bool Equals(Team other) => Id == other.Id;

        public override string ToString() => Id.ToString();
    }
}
