using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Gratify.Api.Services;
using Microsoft.EntityFrameworkCore;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Modals
{
    public class ShowAppHome
    {
        private readonly string _sendGrats = $"{typeof(ShowAppHome)}.SendGrats";
        private readonly string _addTeamMember = $"{typeof(ShowAppHome)}.AddTeamMember";
        private readonly string _removeTeamMember = $"{typeof(ShowAppHome)}.RemoveTeamMember";
        private readonly string _changeSettings = $"{typeof(ShowAppHome)}.ChangeSettings";
        private readonly InteractionService _interactions;
        private readonly GratsDb _database;

        public ShowAppHome(InteractionService interactions, GratsDb database)
        {
            _interactions = interactions;
            _database = database;
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
                            id: _sendGrats,
                            value: userId,
                            text: ":grats: Send Grats")
                    }),

                new Section(
                    id: "YourTeam",
                    text: ":rocket: *Your team*",
                    accessory: new Button(
                        id: _addTeamMember,
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
            if (action.ActionId == _addTeamMember)
            {
                await _interactions.OpenAddTeamMember(triggerId);
            }
            else if (action.ActionId == _removeTeamMember)
            {
                var memberUserId = int.Parse(action.Value);
                await _interactions.RemoveTeamMember(teamId, userId, memberUserId);
            }
            else if (action.ActionId == _sendGrats)
            {
                var draft = new Draft(
                    correlationId: System.Guid.NewGuid(),
                    teamId: teamId,
                    createdAt: System.DateTime.UtcNow,
                    author: userId);

                await _interactions.SendGrats(draft, triggerId);
            }
            else if (action.ActionId == _changeSettings)
            {
                await _interactions.OpenChangeSettings(triggerId, teamId, userId);
            }
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
            var settings = await _interactions.FindSettings(teamId, userId);

            return new List<LayoutBlock>
            {
                new Section(
                    id: "SettingsHeader",
                    text: ":hammer_and_wrench: *Settings*",
                    accessory: new Button(
                        id: _changeSettings,
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
