using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Slack.Client.Interactions;

namespace Gratify.Api.Components.Messages
{
    public class GratsReceived
    {
        private readonly string _transferToAccount = $"{typeof(GratsReceived)}.Yes";
        private readonly string _changeAccountDetails = $"{typeof(GratsReceived)}.ChangeAccountDetails";
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public GratsReceived(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
        }

        public PostMessage Message(Approval approval) =>
            new PostMessage(
                text: $"Congratulations! <@{approval.Review.Grats.Author}> just sent you grats 🎉",
                blocks: new LayoutBlock[]
                {
                    new Section(
                        id: "GratsReceived",
                        text: "*You've Got Grats* :mailbox:"),

                    new Divider(),

                    new Section(
                        id: "Congratulations",
                        text: $"Congratulations! <@{approval.Review.Grats.Author}> just sent you grats 🎉"),

                    new Section(
                        id: "Challenge",
                        text: $"_*Challenge:*_ _{approval.Review.Grats.Challenge}_"),

                    new Section(
                        id: "Action",
                        text: $"_*Action:*_ _{approval.Review.Grats.Action}_"),

                    new Section(
                        id: "Result",
                        text: $"_*Result:*_ _{approval.Review.Grats.Result}_"),

                    new Section(
                        id: "AccountInformation",
                        text: "Would you like kr 1500;- to be transferred to your Vipps account using phone number 413 10 992?"),

                    new Divider(),

                    new Actions(
                        id: "TransferOrChangeDetails",
                        elements: new Button[]
                        {
                            new Button(
                                id: _transferToAccount,
                                correlationId: approval.CorrelationId,
                                text: "Yes!",
                                style: ButtonStyle.Primary),

                            new Button(
                                id: _changeAccountDetails,
                                correlationId: approval.CorrelationId,
                                text: "I would like to change my account details first."),
                        }),
                });

        public async Task OnSubmit(Action action, string triggerId)
        {
            var receival = new Receival(
                correlationId: action.CorrelationId,
                receivedAt: System.DateTime.UtcNow);

            if (action.ActionId == _transferToAccount)
            {
                await TransferToAccount(receival);
            }
            else if (action.ActionId == _changeAccountDetails)
            {
                await ChangeAccountDetails(receival);
            }
        }

        public async Task TransferToAccount(Receival receival) => await ChangeAccountDetails(receival);

        public async Task ChangeAccountDetails(Receival receival)
        {
            var approval = await _database.Approvals
                .Include(approval => approval.Review)
                    .ThenInclude(review => review.Grats)
                .SingleOrDefaultAsync(approval => approval.CorrelationId == receival.CorrelationId);

            if (approval == default)
            {
                _telemetry.TrackEntity($"{nameof(ChangeAccountDetails)}: Approval not found", approval);
                return;
            }

            receival.Approval = approval;
            receival.TeamId = approval.TeamId;

            var notifyReviewer = _components.RequestGratsReview.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyReviewer);

            var notifyAuthor = _components.NotifyGratsSent.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyAuthor);
        }
    }
}
