using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Services;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Slack.Client.Interactions;
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

        public Modal Modal(Review review) =>
            new Modal(
                id: typeof(ForwardGrats),
                correlationId: review.CorrelationId,
                title: "Forward Grats",
                submit: "Forward Grats",
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
            await _interactions.ForwardReview(
                correlationId: submission.CorrelationId,
                newReviewerId: newReviewer.SelectedUserId,
                transferReviewResponsibility: transferResponsibility.SelectedOptions?.Any(option => option == Option.Yes));

            return new ResponseActionClose();
        }
    }
}
