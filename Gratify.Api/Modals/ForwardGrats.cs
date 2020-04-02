using System.Threading.Tasks;
using Gratify.Api.Services;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Primitives;
using Slack.Client.Views;

namespace Gratify.Api.Modals
{
    public class ForwardGrats
    {
        private readonly InteractionService _interactions;

        public ForwardGrats(InteractionService interactions)
        {
            _interactions = interactions;
        }

        public Modal Draw(BlockActions interaction) =>
            new Modal(
                id: typeof(ForwardGrats),
                title: "Forward Grats",
                submit: "Forward Grats",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "SelectApprover",
                        label: "Who should approve Grats?",
                        element: new UsersSelect(
                            id: "GratsApprover",
                            placeholder: "Select a user")),

                    new Section(
                        id: "TransferOptions",
                        text: "Transfer approval responsibility permanently?",
                        accessory: new RadioButtonGroup(
                            id: "TransferApprovalResponsibility",
                            options: new Option[] { Option.No, Option.Yes })),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var newApprover = submission.GetStateValue<User>("SelectApprover.GratsApprover");
            if (newApprover == User.Slackbot)
            {
                return new ResponseActionErrors("SelectReceiver", "Slackbot is not a valid user");
            }

            var transferResponsibility = submission.GetStateValue<string>("TransferOptions.TransferApprovalResponsibility");
            await _interactions.TransferGrats(
                gratsId: submission.GratsId,
                newApprover: newApprover,
                shouldTransferResponsibility: transferResponsibility == Option.Yes.Value);

            return new ResponseActionClose();
        }
    }
}
