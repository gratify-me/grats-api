using System;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Components.Modals
{
    public class SendGrats
    {
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public SendGrats(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
        }

        public Modal Modal(Guid correlationId, string userId = null) =>
            new Modal(
                id: typeof(SendGrats),
                correlationId: correlationId,
                title: $"Send Grats!",
                submit: "Send Grats",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "InputRecipient",
                        label: "Who should receive Grats?",
                        element: new UsersSelect(
                            id: "Recipient",
                            placeholder: "Select a user",
                            initialUser: userId)),

                    new Input(
                        id: "InputChallenge",
                        label: "Challenge",
                        element: new PlainTextInput(
                            id: "Challenge",
                            placeholder: "Describe a situation or a task that your coworker needed to accomplish and where there were some difficulties to overcome.")),

                    new Input(
                        id: "InputAction",
                        label: "Action",
                        element: new PlainTextInput(
                            id: "Action",
                            placeholder: "Describe the actions your coworker took.")),

                    new Input(
                        id: "InputResult",
                        label: "Result",
                        element: new PlainTextInput(
                            id: "Result",
                            placeholder: "What happened? How did the event or situation end? What did you accomplish or learn?")),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var recipient = submission.GetStateValue<UsersSelect>("InputRecipient.Recipient");
            if (recipient.SelectedUser == submission.User)
            {
                return new ResponseActionErrors("InputRecipient", "You cannot send grats to yourself");
            }

            var recipientUser = await _database.Users.SingleAsync(user => user.UserId == recipient.SelectedUser.Id);
            if (recipientUser.IsAdministrator)
            {
                return new ResponseActionErrors("InputRecipient", "You cannot send grats to an Administrator");
            }
            else if (!recipientUser.IsEligibleForGrats)
            {
                return new ResponseActionErrors("InputRecipient", "You cannot send grats to someone who is not eligable to receive Grats");
            }
            else if (recipientUser.HasReports)
            {
                return new ResponseActionErrors("InputRecipient", "You cannot send grats to a Reviewer");
            }

            var challenge = submission.GetStateValue<PlainTextInput>("InputChallenge.Challenge");
            if (challenge.Value.Length > 300)
            {
                return new ResponseActionErrors("InputChallenge", "Challenge cannot be longer than 300 letters");
            }

            var action = submission.GetStateValue<PlainTextInput>("InputAction.Action");
            if (action.Value.Length > 300)
            {
                return new ResponseActionErrors("InputAction", "Action cannot be longer than 300 letters");
            }

            var result = submission.GetStateValue<PlainTextInput>("InputResult.Result");
            if (result.Value.Length > 300)
            {
                return new ResponseActionErrors("InputResult", "Result cannot be longer than 300 letters");
            }

            var grats = new Grats(
                correlationId: submission.CorrelationId,
                teamId: submission.Team.Id,
                createdAt: DateTime.UtcNow,
                recipient: recipient.SelectedUserId,
                author: submission.User.Id,
                challenge: challenge.Value,
                action: action.Value,
                result: result.Value);

            await _database.AddAsync(grats);
            await _database.SaveChangesAsync();

            return new ResponseActionClear();
        }

        public async Task Open(string triggerId, string teamId, string authorId, string userId = null)
        {
            var correlationId = Guid.NewGuid();
            var settings = await _database.SettingsFor(teamId, authorId);
            // TODO: Include pending grats as well to avoid over-sending.
            var approvedGratsLastPeriod = _database.Approvals
                .Select(approval => approval.Review.Grats)
                .Where(grats => grats.Author == authorId)
                .Where(grats => grats.CreatedAt > DateTime.UtcNow.AddDays(-settings.GratsPeriodInDays))
                .OrderByDescending(grats => grats.CreatedAt);

            if (await approvedGratsLastPeriod.CountAsync() >= settings.NumberOfGratsPerPeriod)
            {
                var lastApprovedGrats = await approvedGratsLastPeriod.FirstAsync();
                var allGratsSpentModal = _components.AllGratsSpent.Modal(correlationId, lastApprovedGrats.CreatedAt, settings.GratsPeriodInDays);
                await _slackService.OpenModal(triggerId, allGratsSpentModal);
            }
            else
            {
                var modal = Modal(correlationId, userId);
                await _slackService.OpenModal(triggerId, modal);
            }
        }
    }
}
