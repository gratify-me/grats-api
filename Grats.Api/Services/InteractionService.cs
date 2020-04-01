using System.Threading.Tasks;
using Gratify.Grats.Api.Database;
using Gratify.Grats.Api.Messages;
using Gratify.Grats.Api.Modals;
using Slack.Client.Events;
using Slack.Client.Primitives;

namespace Gratify.Grats.Api.Services
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

        public async Task TransferGrats(string gratsId, User newApprover, bool shouldTransferResponsibility)
        {
            await Task.CompletedTask;
        }

        public async Task UpdateHomeTab(User user)
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
            var approver = await _database.Users.FindAsync(draft.Sender.Id);
            var approverId = approver?.GratsApprover ?? draft.Sender.Id;
            var grats = new Database.Grats
            {
                Sender = draft.Sender.Id,
                Content = draft.Content,
                Approver = approverId,
                Receiver = draft.Receiver.Id,
            };

            await _database.Grats.AddAsync(grats);
            await _database.SaveChangesAsync();

            var channel = await _slackService.GetAppChannel(grats.Approver);
            var approveGrats = new ApproveGratsMessage(grats, channel);
            var blocks = approveGrats.Draw();

            await _slackService.SendMessage(blocks);
        }
    }
}