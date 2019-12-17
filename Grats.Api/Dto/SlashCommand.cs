using Microsoft.AspNetCore.Mvc;

namespace Gratify.Grats.Api.Dto
{
    public class SlashCommand
    {
        [FromForm(Name="token")]
        public string Token { get; set; } // "my-test-token"

        [FromForm(Name="team_id")]
        public string TeamId { get; set; } // "TN27G090U"

        [FromForm(Name="team_domain")]
        public string TeamDomain { get; set; } // "project-gratify"

        [FromForm(Name="channel_id")]
        public string ChannelId { get; set; } // "DMR7GSG11"

        [FromForm(Name="channel_name")]
        public string ChannelName { get; set; } // "directmessage"

        [FromForm(Name="user_id")]
        public string UserId { get; set; } // "UN4H54SDD"

        [FromForm(Name="user_name")]
        public string UserName { get; set; } // "teodor"

        [FromForm(Name="command")]
        public string Command { get; set; } // "%2Fboble"

        [FromForm(Name="text")]
        public string Text { get; set; } // ""

        [FromForm(Name="response_url")]
        public string ResponseUrl { get; set; } // "https%3A%2F%2Fhooks.slack.com%2Fcommands%2FTN27G090U%2F864672117891%2FXFvyDxvxcFFBfYLV5uZHVTmO"

        [FromForm(Name="trigger_id")]
        public string TriggerId { get; set; } // "876149886245.750254009028.f06da6502e157c0990eb996fb2325cdc"
    }
}
