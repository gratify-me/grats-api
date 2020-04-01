using System.Threading.Tasks;
using Gratify.Grats.Api.Database;
using Gratify.Grats.Api.Dto;
using Gratify.Grats.Api.Services;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Primitives;
using Slack.Client.Views;

namespace Gratify.Grats.Api.Modals
{
    public class SendGrats
    {
        private readonly InteractionService _interactions;

        public SendGrats(InteractionService interactions)
        {
            _interactions = interactions;
        }

        public Modal Draw(SlashCommand comand) =>
            new Modal(
                id: typeof(SendGrats),
                title: $"Send grats {comand.UserName}!",
                submit: "Send Grats",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "SelectReceiver",
                        label: "Who should receive Grats?",
                        element: new UsersSelect(
                            id: "GratsReceiver",
                            placeholder: "Select a user")),

                    new Input(
                        id: "WriteGrats",
                        label: "Why should they receive Grats?",
                        element: new PlainTextInput(
                            id: "GratsMessage",
                            placeholder: "A short and concrete description")),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var receiver = submission.GetStateValue<UsersSelect>("SelectReceiver.GratsReceiver");
            if (receiver.SelectedUser == User.Slackbot)
            {
                return new ResponseActionErrors("SelectReceiver", "Slackbot is not a valid user");
            }

            var message = submission.GetStateValue<PlainTextInput>("WriteGrats.GratsMessage");
            if (message.Value.Length > 500)
            {
                return new ResponseActionErrors("WriteGrats", "Grats cannot be longer than 500 letters");
            }

            await _interactions.RequestGratsApproval(new Draft
            {
                Sender = submission.User,
                Receiver = receiver.SelectedUser,
                Content = message.Value,
            });

            return new ResponseActionClose();
        }
    }
}
