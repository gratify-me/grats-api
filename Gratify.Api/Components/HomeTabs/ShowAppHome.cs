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
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Views;
using Action = Slack.Client.Interactions.Action;

namespace Gratify.Api.Components.HomeTabs
{
    public class ShowAppHome
    {
        private readonly string _openSendGrats = $"{typeof(ShowAppHome)}.OpenSendGrats";
        private readonly string _openSendFeedback = $"{typeof(ShowAppHome)}.OpenSendFeedback";
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
            var user = await _database.Users.SingleAsync(user => user.UserId == userId);
            var keyMetrics = await KeyMetrics();
            var yourTeamSection = YourTeamSection(user);
            var teamReviewerSection = TeamReviewerSection(user);
            var teamMembersSection = await TeamMembersSection(user);
            var settingsSection = await SettingsSection(user);
            var homeActions = new Actions(
                id: "HomeActions",
                elements: new BlockElement[]
                {
                    new Button(
                        id: _openSendGrats,
                        value: userId,
                        text: ":grats: Send Grats"),

                    new Button(
                        id: _openSendFeedback,
                        value: userId,
                        text: ":email: Send Feedback")
                });

            return new HomeTab
            {
                Blocks = new List<LayoutBlock>()
                    .Append(keyMetrics)
                    .Append(homeActions)
                    .Append(yourTeamSection)
                    .Append(new Divider())
                    .Append(teamReviewerSection)
                    .Concat(teamMembersSection)
                    .Concat(settingsSection)
                    .ToArray(),
            };
        }

        public async Task OnSubmit(Action action, string triggerId, string userId, string teamId)
        {
            if (action.ActionId == _openAddTeamMember)
            {
                await OpenAddTeamMember(triggerId, teamId, userId);
            }
            else if (action.ActionId == _removeTeamMember)
            {
                var memberUserId = int.Parse(action.Value);
                await RemoveTeamMember(teamId, userId, memberUserId);
            }
            else if (action.ActionId == _openSendGrats)
            {
                await _components.SendGrats.Open(triggerId, teamId, userId);
            }
            else if (action.ActionId == _openChangeSettings)
            {
                await OpenChangeSettings(triggerId, teamId, userId);
            }
            else if (action.ActionId == _openSendFeedback)
            {
                await _components.SendFeedback.Open(triggerId);
            }
        }

        public async Task DisplayFor(string teamId, string userId)
        {
            var homeBlocks = await HomeTab(teamId, userId);
            await _slackService.PublishModal(userId, homeBlocks);
        }

        private async Task OpenAddTeamMember(string triggerId, string teamId, string userId)
        {
            var user = await _database.Users.SingleAsync(user => user.UserId == userId);
            var modal = _components.AddTeamMember.Modal(user);
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

        private async Task<Header> KeyMetrics()
        {
            var noOfGratsSent = await _database.Grats.CountAsync();
            var noOfApprovedGrats = await _database.Approvals.CountAsync();
            var approvedPercentage = noOfGratsSent == 0 ? 0 : Math.Round(noOfApprovedGrats / (double)noOfGratsSent * 100.0);
            var amountOfMoneyReceived = await _database.Receivals.SumAsync(receival => receival.AmountReceived);

            return new Header($":grats: {noOfGratsSent} Grats Sent ({approvedPercentage}% approved)\n:moneybag: {amountOfMoneyReceived} kr received");
        }

        private Section YourTeamSection(User user)
        {
            if (user.IsAdministrator || user.HasReports)
            {
                return new Section(
                    id: "YourTeam",
                    text: ":rocket: *Your team*",
                    accessory: new Button(
                        id: _openAddTeamMember,
                        value: user.UserId,
                        text: ":heavy_plus_sign: New member"));
            }

            return new Section(
                id: "YourTeam",
                text: ":rocket: *Your team*");
        }

        private Section TeamReviewerSection(User currentUser)
        {
            if (currentUser.IsAdministrator)
            {
                // TODO: Administrators should be able to browse different teams, so this will change.
                return new Section(
                    id: currentUser.UserId,
                    text: $"*<@{currentUser.UserId}>* _administrator_");
            }
            else if (currentUser.HasReports)
            {
                return new Section(
                    id: currentUser.UserId,
                    text: $"*<@{currentUser.UserId}>* _reviewer_");
            }
            else if (currentUser.DefaultReviewer != null)
            {
                return new Section(
                    id: currentUser.DefaultReviewer,
                    text: $"*<@{currentUser.DefaultReviewer}>* _reviewer_");
            }

            return new Section(
                id: currentUser.UserId,
                text: $"*<@{currentUser.UserId}>* _not in any team_");
        }

        private async Task<List<Section>> TeamMembersSection(User currentUser)
        {
            var teamMembers = await TeamMembersFor(currentUser);

            return teamMembers
                .Select(user => TeamMemberSection(currentUser, user))
                .ToList();
        }

        private async Task<User[]> TeamMembersFor(User currentUser)
        {
            if (currentUser.IsAdministrator || currentUser.HasReports)
            {
                return await _database.Users
                    .Where(user => user.DefaultReviewer == currentUser.UserId)
                    .ToArrayAsync();
            }

            return await _database.Users
                .Where(user => user.DefaultReviewer != null)
                .Where(user => user.DefaultReviewer == currentUser.DefaultReviewer)
                .ToArrayAsync();
        }

        private Section TeamMemberSection(User currentUser, User user)
        {
            if (currentUser.IsAdministrator || currentUser.HasReports)
            {
                return new Section(
                    id: user.UserId,
                    text: $"*<@{user.UserId}>*{(user.HasReports ? $" _reviewer_" : string.Empty)}",
                    accessory: new Button(
                        id: _removeTeamMember,
                        value: user.Id.ToString(),
                        text: ":heavy_multiplication_x:",
                        style: ButtonStyle.Danger));
            }

            return new Section(
                id: user.UserId,
                text: $"*<@{user.UserId}>*");
        }

        private async Task<List<LayoutBlock>> SettingsSection(User currentUser)
        {
            var settings = await _database.SettingsFor(currentUser.TeamId, currentUser.UserId);

            return new List<LayoutBlock>
            {
                SettingsHeaderSection(currentUser),

                new Divider(),

                new Section(id: "SettingsContent", text: PredefinedSetting.From(settings).Description()),
            };
        }

        private Section SettingsHeaderSection(User currentUser)
        {
            if (currentUser.IsAdministrator)
            {
                return new Section(
                    id: "SettingsHeader",
                    text: ":hammer_and_wrench: *Settings*",
                    accessory: new Button(
                        id: _openChangeSettings,
                        value: currentUser.UserId,
                        text: ":currency_exchange: Change Settings"));
            }

            return new Section(
                id: "SettingsHeader",
                text: ":hammer_and_wrench: *Settings*");
        }
    }
}
