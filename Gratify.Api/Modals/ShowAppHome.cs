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
        private readonly InteractionService _interactions;
        private readonly GratsDb _database;

        public ShowAppHome(InteractionService interactions, GratsDb database)
        {
            _interactions = interactions;
            _database = database;
        }

        public async Task<HomeTab> HomeTab(string userId)
        {
            var teamMemberUsers = await _database.Users
                .Where(user => user.DefaultReviewer == userId)
                .ToArrayAsync();

            var teamMembers = teamMemberUsers
                .Select(user => TeamMemberSection(user))
                .ToList();

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
                Blocks = homeBlocks.Concat(teamMembers).ToArray(),
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
                await _interactions.RemoveTeamMember(userId, memberUserId);
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
    }
}
