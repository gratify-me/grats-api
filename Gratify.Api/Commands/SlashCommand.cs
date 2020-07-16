using Microsoft.AspNetCore.Mvc;

namespace Gratify.Api.Commands
{
    /// <summary>
    /// Slash Commands allow users to invoke your app by typing a string into the message composer box.
    /// A submitted Slash Command will cause a payload of data to be sent from Slack to the associated app.
    /// The app can then respond in whatever way it wants using the context provided by that payload.
    /// When a slash command is invoked, Slack sends an HTTP POST to the Request URL you specified above.
    /// This request contains a data payload describing the source command and who invoked it,
    /// like a really detailed knock at the door.
    /// https://api.slack.com/interactivity/slash-commands
    /// </summary>
    public class SlashCommand
    {
        /// <summary>
        /// The command that was typed in to trigger this request.
        /// This value can be useful if you want to use a single Request URL to service multiple Slash Commands,
        /// as it lets you tell them apart.
        /// </summary>
        /// <example>%2Fboble</example>
        [FromForm(Name = "command")]
        public string Command { get; set; }

        /// <summary>
        /// This is the part of the Slash Command after the command itself,
        /// and it can contain absolutely anything that the user might decide to type.
        /// It is common to use this text parameter to provide extra context for the command.
        /// You can prompt users to adhere to a particular format by showing them in the Usage Hint field when creating a command.
        /// </summary>
        /// <example>any text will do</example>
        [FromForm(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// A temporary webhook URL that you can use to generate messages responses.
        /// </summary>
        /// <example>https%3A%2F%2Fhooks.slack.com%2Fcommands%2FTN27G090U%2F864672117891%2FXFvyDxvxcFFBfYLV5uZHVTmO</example>
        [FromForm(Name = "response_url")]
        public string ResponseUrl { get; set; }

        /// <summary>
        /// A short-lived ID that will let your app open a modal.
        /// </summary>
        /// <example>876149886245.750254009028.f06da6502e157c0990eb996fb2325cdc</example>
        [FromForm(Name = "trigger_id")]
        public string TriggerId { get; set; }

        /// <summary>
        /// Provide context about where the user was in Slack when they triggered your app's command.
        /// </summary>
        /// <example>TN27G090U</example>
        [FromForm(Name="team_id")]
        public string TeamId { get; set; }

        /// <summary>
        /// Provide context about where the user was in Slack when they triggered your app's command.
        /// </summary>
        /// <example>project-gratify</example>
        [FromForm(Name="team_domain")]
        public string TeamDomain { get; set; }

        /// <summary>
        /// Provide context about where the user was in Slack when they triggered your app's command.
        /// </summary>
        /// <example>DMR7GSG11</example>
        [FromForm(Name="channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// Provide context about where the user was in Slack when they triggered your app's command.
        /// </summary>
        /// <example>directmessage</example>
        [FromForm(Name="channel_name")]
        public string ChannelName { get; set; }

        /// <summary>
        /// Provide context about where the user was in Slack when they triggered your app's command.
        /// </summary>
        /// <example>UN4H54SDD</example>
        [FromForm(Name="user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Provide context about where the user was in Slack when they triggered your app's command.
        /// </summary>
        /// <example>teodor</example>
        [FromForm(Name="user_name")]
        public string UserName { get; set; }
    }
}
