using System;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Components.Modals
{
    public class DenyGrats
    {
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public DenyGrats(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
        }

        public Modal Modal(Review review) =>
            new Modal(
                id: typeof(DenyGrats),
                correlationId: review.CorrelationId,
                title: $"Deny Grats",
                submit: "Deny Grats",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "InputReasonForDenial",
                        label: "Reason for denial",
                        element: new PlainTextInput(
                            id: "ReasonForDenial",
                            placeholder: "Why are you denying this Grats?")),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var reason = submission.GetStateValue<PlainTextInput>("InputReasonForDenial.ReasonForDenial");
            if (reason.Value.Length > 500)
            {
                return new ResponseActionErrors("InputResult", "Reason for denial cannot be longer than 500 letters");
            }

            var denial = new Denial(
                correlationId: submission.CorrelationId,
                reason: reason.Value,
                deniedAt: DateTime.UtcNow);

            await SubmitDenyGrats(denial);

            return new ResponseActionClose();
        }

        private async Task SubmitDenyGrats(Denial denial)
        {
            var review = await _database.IncompleteReviews.SingleOrDefaultAsync(review => review.CorrelationId == denial.CorrelationId);
            if (review == default)
            {
                _telemetry.TrackEntity($"{nameof(DenyGrats)}: Denial not found", denial);
                return;
            }

            denial.Review = review;
            denial.TeamId = review.TeamId;
            await _database.AddAsync(denial);
            await _database.SaveChangesAsync();

            var notifyReviewer = _components.RequestGratsReview.UpdateDenied(denial);
            await _slackService.UpdateMessage(notifyReviewer);
        }
    }
}
