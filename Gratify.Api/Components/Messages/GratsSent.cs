using System.Collections.Generic;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Slack.Client;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;

namespace Gratify.Api.Components.Messages
{
    public class GratsSent
    {
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;

        public GratsSent(TelemetryClient telemetry, SlackService slackService)
        {
            _telemetry = telemetry;
            _slackService = slackService;
        }

        public async Task<PostMessage> Message(Grats grats)
        {
            var blocks = new List<LayoutBlock>
            {
                new Section(
                    id: "GratsSent",
                    text: "*Grats Sent* :email:"),

                new Divider(),

                new Section(
                    id: "GratsInformation",
                    text: GratsInformation(grats.Recipient)),

                new Section(
                    id: "Challenge",
                    text: $"_*Challenge:*_ _{grats.Challenge}_"),

                new Section(
                    id: "Action",
                    text: $"_*Action:*_ _{grats.Action}_"),

                new Section(
                    id: "Result",
                    text: $"_*Result:*_ _{grats.Result}_"),

                new Divider(),

                new Context(new MrkdwnText[]
                {
                    new MrkdwnText($":hourglass_flowing_sand: Finding reviewer")
                }),
            };

            var channel = await _slackService.GetAppChannel(grats.Author);
            return new PostMessage(
                text: GratsInformation(grats.Recipient),
                blocks: blocks.ToArray(),
                channel: channel);
        }

        public UpdateMessage UpdatePendingApproval(Review review) =>
            UpdateMessage(review, new MrkdwnText[]
            {
                new MrkdwnText($":hourglass_flowing_sand: Pending review by <@{review.Reviewer}>")
            });

        public UpdateMessage UpdateApproved(Approval approval) =>
            UpdateMessage(approval.Review, new MrkdwnText[]
            {
                new MrkdwnText($":heavy_check_mark: Approved by <@{approval.Review.Reviewer}>")
            });

        public UpdateMessage UpdateDenied(Denial denial) =>
            UpdateMessage(denial.Review, new MrkdwnText[]
            {
                new MrkdwnText($":heavy_multiplication_x: Denied by <@{denial.Review.Reviewer}> _{denial.Reason}_")
            });

        public UpdateMessage UpdateForwarded(Review review) =>
            UpdateMessage(review, new MrkdwnText[]
            {
                new MrkdwnText($":fast_forward: Forwarded review to <@{review.Reviewer}>")
            });

        public UpdateMessage UpdateReceived(Receival receival) =>
            UpdateMessage(receival.Approval.Review, new MrkdwnText[]
            {
                new MrkdwnText($"Grats received! Thanks <@{receival.Approval.Review.Grats.Author}> :heart:")
            });

        private UpdateMessage UpdateMessage(Review review, MrkdwnText[] updates)
        {
            var blocks = new List<LayoutBlock>
            {
                new Section(
                    id: "GratsSent",
                    text: "*Grats Sent* :email:"),

                new Divider(),

                new Section(
                    id: "GratsInformation",
                    text: GratsInformation(review.Grats.Recipient)),

                new Section(
                    id: "Challenge",
                    text: $"_*Challenge:*_ _{review.Grats.Challenge}_"),

                new Section(
                    id: "Action",
                    text: $"_*Action:*_ _{review.Grats.Action}_"),

                new Section(
                    id: "Result",
                    text: $"_*Result:*_ _{review.Grats.Result}_"),

                new Divider(),

                new Context(updates),
            };

            return new UpdateMessage(
                text: GratsInformation(review.Grats.Recipient),
                blocks: blocks.ToArray(),
                originalMessage: review.AuthorNotification);
        }

        private string GratsInformation(string recipient) => $"You've just sent grats to <@{recipient}> :tada:";
   }
}
