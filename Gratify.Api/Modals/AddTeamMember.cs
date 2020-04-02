using System;
using System.Threading.Tasks;
using Gratify.Api.Services;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Primitives;
using Slack.Client.Views;

namespace Gratify.Api.Modals
{
    public class AddTeamMember
    {
        private readonly InteractionService _interactions;

        public AddTeamMember(InteractionService interactions)
        {
            _interactions = interactions;
        }

        public Modal Modal() =>
            new Modal(
                id: typeof(AddTeamMember),
                correlationId: Guid.NewGuid(),
                title: "New member",
                submit: "Add member",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "SelectUser",
                        label: ":heavy_plus_sign: Who should we add to your team?",
                        element: new UsersSelect(
                            id: "NewTeamMemeber",
                            placeholder: "Select a user")),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var newTeamMember = submission.GetStateValue<UsersSelect>("SelectUser.NewTeamMemeber");
            if (newTeamMember.SelectedUser == User.Slackbot)
            {
                return new ResponseActionErrors("SelectReceiver", "Slackbot is not a valid user");
            }

            await _interactions.AddTeamMember(
                teamId: submission.Team.Id,
                userId: submission.User.Id,
                teamMemberId: newTeamMember.SelectedUserId);

            return new ResponseActionClose();
        }
    }
}
