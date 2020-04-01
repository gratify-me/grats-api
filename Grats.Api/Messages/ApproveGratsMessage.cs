using Gratify.Grats.Api.GratsActions;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Slack.Client.Primitives;

namespace Gratify.Grats.Api.Messages
{
    public class ApproveGratsMessage
    {
        private readonly Database.Grats _grats;
        private readonly Channel _channel;

        public ApproveGratsMessage(Database.Grats grats, Channel channel)
        {
            _grats = grats;
            _channel = channel;
        }

        public PostMessage Draw() =>
            new PostMessage
            {
                Channel = _channel.Id,
                Text = $"<@{_grats.Sender}> wants to send grats to <@{_grats.Receiver}>!",
                Blocks = new LayoutBlock[]
                {
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"<@{_grats.Sender}> wants to send grats to <@{_grats.Receiver}>!",
                        },
                    },
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"*Reason:*\n_{_grats.Content}_",
                        },
                    },
                    new Actions
                    {
                        Elements = new Button[]
                        {
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Approve",
                                    Emoji = true,
                                },
                                Style = "primary",
                                Value = ApproveGrats.Name,
                            },
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Deny",
                                    Emoji = true,
                                },
                                Style = "danger",
                                Value = DenyGrats.Name,
                            },
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "Forward",
                                    Emoji = true,
                                },
                                Value = ForwardGrats.Name,
                            },
                        },
                    },
                },
            };
    }
}
