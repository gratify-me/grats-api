using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Gratify.Api.Messages;
using Gratify.Api.Modals;
using Microsoft.EntityFrameworkCore;
using Slack.Client.Events;
using Slack.Client.Primitives;

namespace Gratify.Api.Services
{
    public class InteractionService
    {
        private readonly ISlackService _slackService;
        private readonly GratsDb _database;

        public InteractionService(ISlackService slackService, GratsDb database)
        {
            _slackService = slackService;
            _database = database;
        }

        public async Task RequestGratsApproval(Draft draft)
        {
            await DoRequestGratsApproval(draft);
        }

        public async Task TransferGrats(string gratsId, Slack.Client.Primitives.User newApprover, bool shouldTransferResponsibility)
        {
            await Task.CompletedTask;
        }

        public async Task UpdateHomeTab(Slack.Client.Primitives.User user)
        {
            // TODO: This should have a simpler implementation.
            var appHome = new ShowAppHome(_database, this);
            var appHomeOpened = new AppHomeOpened
            {
                User = user.Id
            };
            var homeBlocks = await appHome.Draw(appHomeOpened);
            await _slackService.PublishModal(user.Id, homeBlocks);
        }

        private async Task DoRequestGratsApproval(Draft draft)
        {
            var approver = await _database.Users
                .Where(user => user.UserId == draft.Author)
                .Where(user => user.TeamId == draft.TeamId)
                .SingleOrDefaultAsync();

            var approverId = approver?.DefaultReviewer ?? draft.Author;
            // TODO: Fix this with DB-service.
            var grats = new Grats();

            await _database.Grats.AddAsync(grats);
            await _database.SaveChangesAsync();

            var channel = await _slackService.GetAppChannel(grats.Recipient);
            var approveGrats = new ApproveGratsMessage(grats, channel);
            var blocks = approveGrats.Draw();

            await _slackService.SendMessage(blocks);
        }
    }
}