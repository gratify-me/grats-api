using System.Threading.Tasks;
using Gratify.Api.Database;
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
        private readonly GratsDb _database;
        private readonly InteractionService _interactions;

        public AddTeamMember(GratsDb database, InteractionService interactions)
        {
            _database = database;
            _interactions = interactions;
        }

        public Modal Draw(BlockActions interaction) =>
            new Modal(
                id: typeof(AddTeamMember),
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
            var newTeamMember = submission.GetStateValue<User>("SelectUser.NewTeamMemeber");
            if (newTeamMember == User.Slackbot)
            {
                return new ResponseActionErrors("SelectReceiver", "Slackbot is not a valid user");
            }

            // TODO: Maybe handle this as an interaction that can fail.
            // I.e. if user is already assigned to a team, show a waring popup.
            var member = await _database.Users.FindAsync(newTeamMember.Id);
            if (member == null)
            {
                member = new Database.Entities.User { UserId = newTeamMember.Id };
                await _database.Users.AddAsync(member);
            }

            member.DefaultReviewer = submission.User.Id;
            await _database.SaveChangesAsync();
            await _interactions.UpdateHomeTab(submission.User);

            // TODO: Something needs to update the HomeView.
            return new ResponseActionClose();
        }
    }
}
