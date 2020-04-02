using Gratify.Api.Database.Entities;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Slack.Client.Primitives;

namespace Gratify.Api.Messages
{
    public class GratsApprovedMessage
    {
        private readonly Receival _receival;
        private readonly Channel _channel;

        public GratsApprovedMessage(Receival receival, Channel channel)
        {
            _receival = receival;
            _channel = channel;
        }

        public PostMessage Draw() =>
            new PostMessage
            {
                Channel = _channel.Id,
                Text = $"Congratulations! <@{_receival.Approval.Review.Grats.Draft.Author}> just sent you grats 🎉",
                Blocks = new LayoutBlock[]
                {
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"Congratulations! <@{_receival.Approval.Review.Grats.Draft.Author}> just sent you grats 🎉",
                        },
                    },
                    new Section
                    {
                        Text = new MrkdwnText
                        {
                            Text = $"*Reason:*\n_{_receival.Approval.Review.Grats.Challenge}_",
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
