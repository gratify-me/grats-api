using System;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Components.Modals
{
    public class ForwardReview
    {
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public ForwardReview(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
        }

        public Modal Modal(Review review) =>
            new Modal(
                id: typeof(ForwardReview),
                correlationId: review.CorrelationId,
                title: "Forward Review",
                submit: "Forward Review",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "InputReviewer",
                        label: "Who should approve Grats?",
                        element: new UsersSelect(
                            id: "Reviewer",
                            placeholder: "Select a user")),

                    new Input(
                        id: "InputTransferReviewResponsibility",
                        label: "Transfer approval responsibility permanently?",
                        element: new CheckboxGroup(
                            id: "TransferReviewResponsibility",
                            options: new Option[] { Option.Yes }),
                        optional: true),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var newReviewer = submission.GetStateValue<UsersSelect>("InputReviewer.Reviewer");
            if (newReviewer.SelectedUser == Slack.Client.Primitives.User.Slackbot)
            {
                return new ResponseActionErrors("SelectReceiver", "Slackbot is not a valid user");
            }

            var transferResponsibility = submission.GetStateValue<CheckboxGroup>("InputTransferReviewResponsibility.TransferReviewResponsibility");
            await ForwardReviewTo(
                correlationId: submission.CorrelationId,
                newReviewerId: newReviewer.SelectedUserId,
                transferReviewResponsibility: transferResponsibility.SelectedOptions?.Any(option => option == Option.Yes));

            return new ResponseActionClose();
        }

        public async Task ForwardReviewTo(Guid correlationId, string newReviewerId, bool? transferReviewResponsibility)
        {
            var oldReview = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == correlationId);
            if (oldReview == default)
            {
                _telemetry.TrackCorrelationId($"{nameof(ForwardReview)}: Review not found", correlationId);
                return;
            }

            if (transferReviewResponsibility.GetValueOrDefault(false))
            {
                // TODO: This might be combined into one query.
                var grats = await _database.Grats.SingleAsync(grats => grats.CorrelationId == correlationId);
                var user = await _database.Users.SingleAsync(user => user.UserId == grats.Recipient);
                user.DefaultReviewer = newReviewerId;
            }

            var newReview = oldReview.ForwardTo(newReviewerId);
            var reviewMessage = await _components.ReviewGrats.Message(newReview);
            newReview.SetReviewRequest(await _slackService.SendMessage(reviewMessage));

            await _database.AddAsync(newReview);
            await _database.SaveChangesAsync();

            var notifyOldReviewer = _components.ReviewGrats.UpdateForwarded(oldReview, newReview);
            await _slackService.UpdateMessage(notifyOldReviewer);
        }
    }
}
