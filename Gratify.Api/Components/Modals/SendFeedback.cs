using System;
using System.Threading.Tasks;
using Gratify.Api.Services;
using Microsoft.ApplicationInsights;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Components.Modals
{
    public class SendFeedback
    {
        private readonly TelemetryClient _telemetry;
        private readonly SlackService _slackService;
        private readonly EmailClient _emailClient;

        public SendFeedback(TelemetryClient telemetry, SlackService slackService, EmailClient emailClient)
        {
            _telemetry = telemetry;
            _slackService = slackService;
            _emailClient = emailClient;
        }

        public Modal Modal(Guid correlationId) =>
            new Modal(
                id: typeof(SendFeedback),
                correlationId: correlationId,
                title: $"Send Feedback",
                submit: "Send",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "InputFeedback",
                        label: "Your feedback",
                        element: new PlainTextInput(
                            id: "UserFeedback",
                            placeholder: "What would you like to tell us?")),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var feedback = submission.GetStateValue<PlainTextInput>("InputFeedback.UserFeedback");
            var mailSent = await _emailClient.SendMail($"Feedback from {submission.User.Id}", feedback.Value);
            if (mailSent)
            {
                return new ResponseActionClose();
            }
            else
            {
                var message = "We're sorry, but something went wrong. Please try again, and if that fails email us at inbox@gratify.no";
                return new ResponseActionErrors("InputFeedback", message);
            }
        }

        public async Task Open(string triggerId)
        {
            var correlationId = Guid.NewGuid();
            var modal = Modal(correlationId);
            await _slackService.OpenModal(triggerId, modal);
        }
    }
}
