using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Views;
using Action = Slack.Client.Interactions.Action;

namespace Gratify.Api.Components.HomeTabs
{
    public class ShowAppHome
    {
        private readonly string _openSendGrats = $"{typeof(ShowAppHome)}.OpenSendGrats";
        private readonly string _openAddTeamMember = $"{typeof(ShowAppHome)}.OpenAddTeamMember";
        private readonly string _removeTeamMember = $"{typeof(ShowAppHome)}.RemoveTeamMember";
        private readonly string _openChangeSettings = $"{typeof(ShowAppHome)}.OpenChangeSettings";
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public ShowAppHome(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
        }

        public async Task<HomeTab> HomeTab(string teamId, string userId)
        {
            var teamMemberUsers = await _database.Users
                .Where(user => user.DefaultReviewer == userId)
                .ToArrayAsync();

            var teamMembers = teamMemberUsers
                .Select(user => TeamMemberSection(user))
                .ToList();

            var settingsSection = await SettingsSection(teamId, userId);

            var homeBlocks = new List<LayoutBlock>
            {
                new Actions(
                    id: "HomeActions",
                    elements: new BlockElement[]
                    {
                        new Button(
                            id: _openSendGrats,
                            value: userId,
                            text: ":grats: Send Grats")
                    }),

                new Section(
                    id: "YourTeam",
                    text: ":rocket: *Your team*",
                    accessory: new Button(
                        id: _openAddTeamMember,
                        value: userId,
                        text: ":heavy_plus_sign: New member")),

                new Divider(),
            };

            return new HomeTab
            {
                Blocks = homeBlocks
                    .Concat(teamMembers)
                    .Concat(settingsSection)
                    .ToArray(),
            };
        }

        public async Task OnSubmit(Action action, string triggerId, string userId, string teamId)
        {
            if (action.ActionId == _openAddTeamMember)
            {
                await OpenAddTeamMember(triggerId);
            }
            else if (action.ActionId == _removeTeamMember)
            {
                var memberUserId = int.Parse(action.Value);
                await RemoveTeamMember(teamId, userId, memberUserId);
            }
            else if (action.ActionId == _openSendGrats)
            {
                var draft = new Draft(
                    correlationId: System.Guid.NewGuid(),
                    teamId: teamId,
                    createdAt: System.DateTime.UtcNow,
                    author: userId);

                await _components.SendGrats.OpenSendGrats(draft, triggerId);
            }
            else if (action.ActionId == _openChangeSettings)
            {
                await OpenChangeSettings(triggerId, teamId, userId);
            }
        }

        public async Task DisplayFor(string teamId, string userId)
        {
            var homeBlocks = await HomeTab(teamId, userId);
            await _slackService.PublishModal(userId, homeBlocks);
        }

        private async Task OpenAddTeamMember(string triggerId)
        {
            var modal = _components.AddTeamMember.Modal();
            await _slackService.OpenModal(triggerId, modal);
        }

        private async Task RemoveTeamMember(string teamId, string userId, int teamMemberId)
        {
            var teamMember = await _database.Users.FindAsync(teamMemberId);
            if (teamMember == null)
            {
                _telemetry.TrackUserId($"{nameof(RemoveTeamMember)}: User not found", userId);
                return;
            }

            teamMember.DefaultReviewer = null;
            await _database.SaveChangesAsync();
            await DisplayFor(teamId, userId);
        }

        private async Task OpenChangeSettings(string triggerId, string teamId, string userId)
        {
            var settings = await _database.SettingsFor(teamId, userId);
            var modal = _components.ChangeSettings.Modal(settings);
            await _slackService.OpenModal(triggerId, modal);
        }

        private Section TeamMemberSection(User user) =>
            new Section(
                id: user.UserId,
                text: $"*<@{user.UserId}>*",
                accessory: new Button(
                    id: _removeTeamMember,
                    value: user.Id.ToString(),
                    text: ":heavy_multiplication_x:",
                    style: ButtonStyle.Danger));

        private async Task<List<LayoutBlock>> SettingsSection(string teamId, string userId)
        {
            var settings = await _database.SettingsFor(teamId, userId);

            return new List<LayoutBlock>
            {
                new Section(
                    id: "SettingsHeader",
                    text: ":hammer_and_wrench: *Settings*",
                    accessory: new Button(
                        id: _openChangeSettings,
                        value: userId,
                        text: ":currency_exchange: Change Settings")),

                new Divider(),

                new Section(
                    id: "SettingsContent",
                    text: $"Grats period in days: *{settings.GratsPeriodInDays}*\n"
                        + $"Number of Grats per period: *{settings.NumberOfGratsPerPeriod}*"),
            };
        }
    }
}
