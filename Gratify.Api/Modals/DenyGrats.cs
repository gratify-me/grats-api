using System;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Services;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Modals
{
    public class DenyGrats
    {
        private readonly InteractionService _interactions;

        public DenyGrats(InteractionService interactions)
        {
            _interactions = interactions;
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

            await _interactions.DenyGrats(denial);

            return new ResponseActionClose();
        }
    }
}
