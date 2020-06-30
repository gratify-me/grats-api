using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Services;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Slack.Client.Interactions;

namespace Gratify.Api.Messages
{
    public class GratsReceived
    {
        private readonly string _transferToAccount = $"{typeof(GratsReceived)}.Yes";
        private readonly string _changeAccountDetails = $"{typeof(GratsReceived)}.ChangeAccountDetails";
        private readonly InteractionService _interactions;

        public GratsReceived(InteractionService interactions = default)
        {
            _interactions = interactions;
        }

        public PostMessage Message(Approval approval) =>
            new PostMessage(
                text: $"Congratulations! <@{approval.Review.Grats.Draft.Author}> just sent you grats 🎉",
                blocks: new LayoutBlock[]
                {
                    new Section(
                        id: "GratsReceived",
                        text: "*You've Got Grats* :mailbox:"),

                    new Divider(),

                    new Section(
                        id: "Congratulations",
                        text: $"Congratulations! <@{approval.Review.Grats.Draft.Author}> just sent you grats 🎉"),

                    new Section(
                        id: "Challenge",
                        text: $"_*Challenge:*_ _{approval.Review.Grats.Challenge}_"),

                    new Section(
                        id: "Action",
                        text: $"_*Action:*_ _{approval.Review.Grats.Action}_"),

                    new Section(
                        id: "Result",
                        text: $"_*Result:*_ _{approval.Review.Grats.Result}_"),

                    new Section(
                        id: "AccountInformation",
                        text: "Would you like kr 1500;- to be transferred to your Vipps account using phone number 413 10 992?"),

                    new Divider(),

                    new Actions(
                        id: "TransferOrChangeDetails",
                        elements: new Button[]
                        {
                            new Button(
                                id: _transferToAccount,
                                correlationId: approval.CorrelationId,
                                text: "Yes!",
                                style: ButtonStyle.Primary),

                            new Button(
                                id: _changeAccountDetails,
                                correlationId: approval.CorrelationId,
                                text: "I would like to change my account details first."),
                        }),
                });

        public async Task OnSubmit(Action action, string triggerId)
        {
            var receival = new Receival(
                correlationId: action.CorrelationId,
                receivedAt: System.DateTime.UtcNow);

            if (action.ActionId == _transferToAccount)
            {
                await _interactions.TransferToAccount(receival);
            }
            else if (action.ActionId == _changeAccountDetails)
            {
                await _interactions.ChangeAccountDetails(receival);
            }
        }
    }
}
