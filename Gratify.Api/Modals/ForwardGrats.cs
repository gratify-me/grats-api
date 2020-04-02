using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Services;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
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

                    new Section(
                        id: "SelectTransferReviewResponsibility",
                        text: "Transfer approval responsibility permanently?",
                        accessory: new RadioButtonGroup(
                            id: "TransferReviewResponsibility",
                            options: new Option[] { Option.No, Option.Yes })),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var newReviewer = submission.GetStateValue<UsersSelect>("InputReviewer.Reviewer");
            if (newReviewer.SelectedUser == Slack.Client.Primitives.User.Slackbot)
            {
                return new ResponseActionErrors("SelectReceiver", "Slackbot is not a valid user");
            }

            var transferResponsibility = submission.GetStateValue<string>("SelectTransferReviewResponsibility.TransferReviewResponsibility");
            var newReview = new Review(
                correlationId: submission.CorrelationId,
                requestedAt: System.DateTime.UtcNow,
                reviewer: newReviewer.SelectedUserId);

            await _interactions.ForwardReview(
                newReview: newReview,
                transferReviewResponsibility: transferResponsibility == Option.Yes.Value);

            return new ResponseActionClose();
        }
    }
}
