using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Action = Slack.Client.Interactions.Action;

namespace Gratify.Api.Components.Messages
{
    public class RequestGratsReview
    {
        private readonly string _approve = $"{typeof(RequestGratsReview)}.Approve";
        private readonly string _deny = $"{typeof(RequestGratsReview)}.Deny";
        private readonly string _forward = $"{typeof(RequestGratsReview)}.Forward";
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public RequestGratsReview(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
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

                await ApproveGrats(approval);
            }
            else if (action.ActionId == _deny)
            {
                await OpenDenyGrats(action.CorrelationId, triggerId);
            }
            else if (action.ActionId == _forward)
            {
                await OpenForwardReview(action.CorrelationId, triggerId);
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

        private async Task ApproveGrats(Approval approval)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == approval.CorrelationId);
            if (review == default)
            {
                _telemetry.TrackEntity($"{nameof(ApproveGrats)}: Approval not found", approval);
                return;
            }

            approval.Review = review;
            approval.TeamId = review.TeamId;
            await _database.AddAsync(approval);
            await _database.SaveChangesAsync();

            var notifyReviewer = UpdateApproved(approval);
            await _slackService.UpdateMessage(notifyReviewer);
        }

        private async Task OpenDenyGrats(Guid correlationId, string triggerId)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == correlationId);
            if (review == default)
            {
                _telemetry.TrackCorrelationId($"{nameof(OpenDenyGrats)}: Review not found", correlationId);
                return;
            }

            var modal = _components.DenyGrats.Modal(review);
            await _slackService.OpenModal(triggerId, modal);
        }

        private async Task OpenForwardReview(Guid correlationId, string triggerId)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == correlationId);
            if (review == default)
            {
                _telemetry.TrackCorrelationId($"{nameof(OpenForwardReview)}: Review not found", correlationId);
                return;
            }

            var modal = _components.ForwardGrats.Modal(review);
            await _slackService.OpenModal(triggerId, modal);
        }
    }
}
