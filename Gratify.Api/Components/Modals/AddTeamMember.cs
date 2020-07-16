using System;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;
using DbUser = Gratify.Api.Database.Entities.User;
using User = Slack.Client.Primitives.User;

namespace Gratify.Api.Components.Modals
{
    public class AddTeamMember
    {
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public AddTeamMember(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
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

            await SaveNewTeamMember(
                teamId: submission.Team.Id,
                userId: submission.User.Id,
                teamMemberId: newTeamMember.SelectedUserId);

            return new ResponseActionClose();
        }

        private async Task SaveNewTeamMember(string teamId, string userId, string teamMemberId)
        {
            var teamMember = await _database.Users.SingleOrDefaultAsync(user => user.UserId == teamMemberId);
            if (teamMember == default)
            {
                teamMember = new DbUser(teamId, teamMemberId, defaultReviewer: userId);
                await _database.Users.AddAsync(teamMember);
            }
            else
            {
                teamMember.DefaultReviewer = userId;
            }

            await _database.SaveChangesAsync();
            var homeBlocks = await _components.ShowAppHome.HomeTab(teamId, userId);
            await _slackService.PublishModal(userId, homeBlocks);
        }
    }
}
