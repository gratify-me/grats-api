using System;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Action = Slack.Client.Interactions.Action;

namespace Gratify.Api.Components.Messages
{
    public class GratsRemaining
    {
        private readonly string _sendGrats = $"{typeof(GratsRemaining)}.SendGrats";
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public GratsRemaining(TelemetryClient telemetry, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _slackService = slackService;
            _components = components;
        }

        public async Task<PostMessage> Message(User user) =>
            new PostMessage(
                text: "You have remaining Grats :bell:",
                blocks: new LayoutBlock[]
                {
                    new Section(
                        id: "GratsRemaining",
                        text: "*You have remaining Grats* :raised_hand:"),

                    new Section(
                        id: "AnyoneDeserveGrats",
                        text: "Know anyone that deserves praise? Send them :grats: and show your love :heart_eyes:"),

                    new Actions(
                        id: "SendGrats",
                        elements: new Button[]
                        {
                            new Button(
                                id: _sendGrats,
                                correlationId: Guid.NewGuid(),
                                text: ":grats: Send Grats",
                                style: ButtonStyle.Primary),
                        }),
                },
                channel: await _slackService.GetAppChannel(user.UserId));

        public async Task OnSubmit(Action action, string triggerId, string userId, string teamId)
        {
            if (action.ActionId == _sendGrats)
            {
                await _components.SendGrats.Open(triggerId, teamId, userId);
            }
        }
    }
}
