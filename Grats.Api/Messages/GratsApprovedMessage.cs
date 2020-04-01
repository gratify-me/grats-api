using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Slack.Client.Primitives;

namespace Gratify.Grats.Api.Messages
{
    public class GratsApprovedMessage
    {
        private readonly Database.Grats _grats;
        private readonly Channel _channel;

        public GratsApprovedMessage(Database.Grats grats, Channel channel)
        {
            _grats = grats;
            _channel = channel;
        }

        public PostMessage Draw() =>
            new PostMessage
            {
                Channel = _channel.Id,
                Text = $"Congratulations! <@{_grats.Sender}> just sent you grats 🎉",
                Blocks = new LayoutBlock[]
                {
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"Congratulations! <@{_grats.Sender}> just sent you grats 🎉",
                        },
                    },
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"*Reason:*\n_{_grats.Content}_",
                        },
                    },
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = "Would you like kr 1500;- to be transferred to your Vipps account using phone number 413 10 992?",
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
                                    Text = "Yes!",
                                    Emoji = true,
                                },
                                Style = "primary",
                                Value = "todo",
                            },
                            new Button
                            {
                                Text = new PlainText
                                {
                                    Text = "No. I would like to change my account details.",
                                    Emoji = true,
                                },
                                Style = "danger",
                                Value = "todo",
                            },
                        },
                    },
                },
            };
    }
}
