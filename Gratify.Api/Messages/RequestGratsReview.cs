using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Services;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Slack.Client.Interactions;

namespace Gratify.Api.Messages
{
    public class RequestGratsReview
    {
        private readonly string _approve = $"{typeof(RequestGratsReview)}.Approve";
        private readonly string _deny = $"{typeof(RequestGratsReview)}.Deny";
        private readonly string _forward = $"{typeof(RequestGratsReview)}.Forward";
        private readonly InteractionService _interactions;
        private readonly SlackService _slackService;

        public RequestGratsReview(SlackService slackService)
        {
            _slackService = slackService;
        }

        public RequestGratsReview(InteractionService interactions, SlackService slackService)
        {
            _interactions = interactions;
            _slackService = slackService;
        }

        public async Task<PostMessage> Message(Review review) =>
            new PostMessage(
                text: WantsToSendGrats(review),
                blocks: BaseBlocks(review)
                    .Append(new Actions(
                        id: "ApproveDenyOrForward",
                        elements: new Button[]
                        {
                            new Button(
                                id: _approve,
                                correlationId: review.CorrelationId,
                                text: "Approve",
                                style: ButtonStyle.Primary),

                            new Button(
                                id: _deny,
                                correlationId: review.CorrelationId,
                                text: "Deny",
                                style: ButtonStyle.Danger),

                            new Button(
                                id: _forward,
                                correlationId: review.CorrelationId,
                                text: "Forward"),
                        }))
                    .ToArray(),
                channel: await _slackService.GetAppChannel(review.Reviewer));

        public UpdateMessage UpdateApproved(Approval approval) =>
            UpdateMessage(approval.Review, new MrkdwnText[]
            {
                new MrkdwnText($":heavy_check_mark: Grats approved!")
            });

        public UpdateMessage UpdateDenied(Denial denial) =>
            UpdateMessage(denial.Review, new MrkdwnText[]
            {
                new MrkdwnText($":heavy_multiplication_x: Grats denied: _{denial.Reason}_")
            });

        public UpdateMessage UpdateForwarded(Review oldReview, Review newReview) =>
            UpdateMessage(oldReview, new MrkdwnText[]
            {
                new MrkdwnText($":fast_forward: Review forwarded to <@{newReview.Reviewer}>")
            });

        public UpdateMessage UpdateReceived(Receival receival) =>
            UpdateMessage(receival.Approval.Review, new MrkdwnText[]
            {
                new MrkdwnText($"Grats received! Thanks <@{receival.Approval.Review.Reviewer}> :heart:")
            });

        public async Task OnSubmit(Action action, string triggerId)
        {
            if (action.ActionId == _approve)
            {
                var approval = new Approval(
                    correlationId: action.CorrelationId,
                    approvedAt: System.DateTime.UtcNow);

                await _interactions.ApproveGrats(approval);
            }
            else if (action.ActionId == _deny)
            {
                await _interactions.OpenDenyGrats(action.CorrelationId, triggerId);
            }
            else if (action.ActionId == _forward)
            {
                await _interactions.OpenForwardReview(action.CorrelationId, triggerId);
            }
        }

        private List<LayoutBlock> BaseBlocks(Review review) =>
            new List<LayoutBlock>
            {
                new Section(
                    id: "ReviewRequested",
                    text: "*Review Requested* :mag:"),

                new Divider(),

                new Section(
                    id: "AuthorAndRecipient",
                    text: WantsToSendGrats(review)),

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
            };

        private UpdateMessage UpdateMessage(Review review, MrkdwnText[] updates) =>
            new UpdateMessage(
                text: WantsToSendGrats(review),
                blocks: BaseBlocks(review)
                    .Append(new Context(updates))
                    .ToArray(),
                originalMessage: review.ReviewRequest);

        private string WantsToSendGrats(Review review) => $"<@{review.Grats.Draft.Author}> wants to send grats to <@{review.Grats.Recipient}>!";
    }
}
