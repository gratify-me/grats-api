using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Gratify.Api.GratsActions;
using Gratify.Api.Services;
using Microsoft.EntityFrameworkCore;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Events;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Modals
{
    public class ShowAppHome
    {
        private readonly GratsDb _database;
        private readonly InteractionService _interactions;

        public ShowAppHome(GratsDb database, InteractionService interactions)
        {
            _database = database;
            _interactions = interactions;
        }

        public async Task<HomeTab> Draw(AppHomeOpened clientEvent)
        {
            var userId = clientEvent.User;
            var teamMemberUsers = await _database.Users
                .Where(user => user.DefaultReviewer == userId)
                .ToArrayAsync();

            var teamMembers = teamMemberUsers
                .Select(user => TeamMemberSection(user))
                .ToList();

            var homeBlocks = new List<LayoutBlock>
            {
                new Section
                {
                    Text = new MrkdwnText
                    {
                        Text = ":rocket: *Your team*",
                    },
                    Accessory = new Button
                    {
                        Text = new PlainText
                        {
                            Text = ":heavy_plus_sign: New member",
                            Emoji = true,
                        },
                        Value = RemoveTeamMember.Name,
                    }
                },
                new Divider(),
            };

            return new HomeTab
            {
                Blocks = homeBlocks.Concat(teamMembers).ToArray(),
            };
        }

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            // TODO: Man har egentlig ikke en submit på Home Tab. Er dette kanskje feil abstraksjon?
            // Er det heller verdt å bygge noe spesielt for Home Tab?
            await Task.CompletedTask;
            return null;
        }

        private Section TeamMemberSection(User user) =>
            new Section
            {
                Text = new MrkdwnText
                {
                    Text = $"*<@{user.Id}>*"
                },
                Accessory = new Button
                {
                    Style = "danger",
                    Text = new PlainText
                    {
                        Text = "Remove",
                        Emoji = true,
                    },
                    Value = RemoveTeamMember.Name,
                }
            };
    }
}
