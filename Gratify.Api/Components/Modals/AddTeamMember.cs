using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

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

        public Modal Modal(User user)
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
            var newTeamMemberInput = submission.GetStateValue<UsersSelect>("InputNewTeamMember.NewTeamMember");
            var newTeamMember = await _database.Users.SingleOrDefaultAsync(user => user.UserId == newTeamMemberInput.SelectedUserId);
            if (!newTeamMember.IsEligibleForGrats)
            {
                return new ResponseActionErrors("InputNewTeamMember", "This user cannor receive Grats, and should not be part of any team.");
            }

            // TODO: Should abstract away this in DbContext. Also, probably need to use TeamId along with UserId to identify users.
            // TODO: Is it a good idea to block this operation entirely?
            var currentUser = await _database.Users.SingleOrDefaultAsync(user => user.UserId == submission.User.Id);
            if (newTeamMember.DefaultReviewer != null && !currentUser.IsAdministrator)
            {
                return new ResponseActionErrors("InputNewTeamMember", $"This user is already part of another team.");
            }

            var userIsManager = submission.GetStateValue<CheckboxGroup>("InputUserIsManager.UserIsManager");
            var userHasReports = userIsManager?.SelectedOptions?.Any(option => option == Option.Yes);

            await SaveNewTeamMember(
                newTeamMember: newTeamMember,
                currentUser: currentUser,
                hasReports: userHasReports ?? false);

            return new ResponseActionClose();
        }

        private async Task SaveNewTeamMember(User newTeamMember, User currentUser, bool hasReports)
        {
            newTeamMember.DefaultReviewer = currentUser.UserId;
            newTeamMember.HasReports = hasReports;

            await _database.SaveChangesAsync();
            var homeBlocks = await _components.ShowAppHome.HomeTab(currentUser.TeamId, currentUser.UserId);
            await _slackService.PublishModal(currentUser.UserId, homeBlocks);
        }
    }
}
