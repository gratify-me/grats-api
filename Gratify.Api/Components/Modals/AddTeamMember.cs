using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
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

        public Modal Modal(DbUser user)
        {
            var blocks = new List<LayoutBlock>
            {
                new Input(
                    id: "InputNewTeamMember",
                    label: ":heavy_plus_sign: Who should we add to your team?",
                    element: new UsersSelect(
                        id: "NewTeamMember",
                        placeholder: "Select a user")),
            };

            if (user.IsAdministrator)
            {
                blocks.Add(new Input(
                    id: "InputUserIsManager",
                    label: "Is user manager?",
                    element: new CheckboxGroup(
                        id: "UserIsManager",
                        options: new Option[] { Option.Yes }),
                    optional: true));
            }

            return new Modal(
                id: typeof(AddTeamMember),
                correlationId: Guid.NewGuid(),
                title: "New member",
                submit: "Add member",
                close: "Cancel",
                blocks: blocks.ToArray());
        }

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var newTeamMember = submission.GetStateValue<UsersSelect>("InputNewTeamMember.NewTeamMember");
            if (newTeamMember.SelectedUser == User.Slackbot)
            {
                return new ResponseActionErrors("SelectReceiver", "Slackbot is not a valid user");
            }

            var userIsManager = submission.GetStateValue<CheckboxGroup>("InputUserIsManager.UserIsManager");
            var userHasReports = userIsManager?.SelectedOptions?.Any(option => option == Option.Yes);
            await SaveNewTeamMember(
                teamId: submission.Team.Id,
                userId: submission.User.Id,
                teamMemberId: newTeamMember.SelectedUserId,
                hasReports: userHasReports ?? false);

            return new ResponseActionClose();
        }

        private async Task SaveNewTeamMember(string teamId, string userId, string teamMemberId, bool hasReports)
        {
            var teamMember = await _database.Users.SingleOrDefaultAsync(user => user.UserId == teamMemberId);
            teamMember.DefaultReviewer = userId;
            teamMember.HasReports = hasReports;

            await _database.SaveChangesAsync();
            var homeBlocks = await _components.ShowAppHome.HomeTab(teamId, userId);
            await _slackService.PublishModal(userId, homeBlocks);
        }
    }
}
