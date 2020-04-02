using System;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Services;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Modals
{
    public class SendGrats
    {
        private readonly InteractionService _interactions;

        public SendGrats(InteractionService interactions)
        {
            _interactions = interactions;
        }

        public Modal Modal(Draft draft) =>
            new Modal(
                id: typeof(SendGrats),
                correlationId: draft.CorrelationId,
                title: $"Send Grats!",
                submit: "Send Grats",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "InputRecipient",
                        label: "Who should receive Grats?",
                        element: new UsersSelect(
                            id: "Recipient",
                            placeholder: "Select a user")),

                    new Input(
                        id: "InputChallenge",
                        label: "Challenge",
                        element: new PlainTextInput(
                            id: "Challenge",
                            placeholder: "Describe a situation or a task that your coworker needed to accomplish and where there were some difficulties to overcome.")),

                    new Input(
                        id: "InputAction",
                        label: "Action",
                        element: new PlainTextInput(
                            id: "Action",
                            placeholder: "Describe the actions your coworker took.")),

                    new Input(
                        id: "InputResult",
                        label: "Result",
                        element: new PlainTextInput(
                            id: "Result",
                            placeholder: "What happened? How did the event or situation end? What did you accomplish or learn?")),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var recipient = submission.GetStateValue<UsersSelect>("InputRecipient.Recipient");
            if (recipient.SelectedUser == Slack.Client.Primitives.User.Slackbot)
            {
                return new ResponseActionErrors("InputRecipient", "Slackbot is not a valid recipient");
            }

            var challenge = submission.GetStateValue<PlainTextInput>("InputChallenge.Challenge");
            if (challenge.Value.Length > 300)
            {
                return new ResponseActionErrors("InputChallenge", "Challenge cannot be longer than 300 letters");
            }

            var action = submission.GetStateValue<PlainTextInput>("InputAction.Action");
            if (action.Value.Length > 300)
            {
                return new ResponseActionErrors("InputAction", "Action cannot be longer than 300 letters");
            }

            var result = submission.GetStateValue<PlainTextInput>("InputResult.Result");
            if (result.Value.Length > 300)
            {
                return new ResponseActionErrors("InputResult", "Result cannot be longer than 300 letters");
            }

            var grats = new Grats(
                correlationId: submission.CorrelationId,
                createdAt: DateTime.UtcNow,
                recipient: recipient.SelectedUserId,
                challenge: challenge.Value,
                action: action.Value,
                result: result.Value);

            await _interactions.SubmitGratsForReview(grats);

            return new ResponseActionClose();
        }
    }
}
