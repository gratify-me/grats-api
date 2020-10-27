using System;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Microsoft.ApplicationInsights;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Components.Modals
{
    public class AllGratsSpent
    {
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly ComponentsService _components;

        public AllGratsSpent(TelemetryClient telemetry, GratsDb database, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _components = components;
        }

        public Modal Modal(Guid correlationId, DateTime lastGratsSentAt, int periodInDays) =>
            new Modal(
                id: typeof(AllGratsSpent),
                correlationId: correlationId,
                title: $"All Grats Spent",
                submit: "Send anyway",
                close: "Ok",
                blocks: new LayoutBlock[]
                {
                    new Section(
                        id: "AllGratsSpent",
                        text: $"*You've spent all your Grats the last {periodInDays} days.*\n"
                            + $"But don't dispair! You'll have new Grats to spend in {RemainingDays(lastGratsSentAt, periodInDays)} days."),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            await Task.CompletedTask;
            var modal = _components.SendGrats.Modal(submission.CorrelationId);

            return new ResponseActionPush(modal);
        }

        private int RemainingDays(DateTime lastGratsSentAt, int periodInDays) =>
            lastGratsSentAt.Subtract(DateTime.UtcNow.AddDays(-periodInDays)).Days;
    }
}
