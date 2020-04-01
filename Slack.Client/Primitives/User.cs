using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Primitives
{
    public class User
    {
        public static User Slackbot => new User
        {
            Id = "USLACKBOT",
        };

        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }

        public override bool Equals(object obj) => obj is User other && Equals(other);

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(User first, User second) => first.Id == second.Id;

        public static bool operator !=(User first, User second) => first.Id != second.Id;

        public bool Equals(User other) => Id == other.Id;

        public override string ToString() => Id.ToString();
    }
}
