using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Components.Modals
{
    public class RegisterAccountDetails
    {
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public RegisterAccountDetails(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
        }

        public Modal Modal(Guid correlationId) =>
            new Modal(
                id: typeof(RegisterAccountDetails),
                correlationId: correlationId,
                title: $"Register Account Number",
                submit: "Submit",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "InputReceiverAccountNumber",
                        label: "Bank account number",
                        element: new PlainTextInput(
                            id: "AccountNumber",
                            placeholder: "11-digit Norwegian bank account number",
                            multiline: false)),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var accountNumberInput = submission.GetStateValue<PlainTextInput>("InputReceiverAccountNumber.AccountNumber");
            var accountNumber = RemoveWhitespace(accountNumberInput.Value);
            if (!IsValidAccountNumber(accountNumber))
            {
                return new ResponseActionErrors("InputReceiverAccountNumber", $"{accountNumberInput.Value} is not a valid Norwegian account number");
            }

            var receival = new Receival(
                correlationId: submission.CorrelationId,
                receivedAt: System.DateTime.UtcNow,
                receiverName: submission.User.RealName, // TODO: We're not guaranteed to get this. Will transfer still work then?
                receiverAccountNumber: accountNumber);

            await SubmitAccountDetails(receival);

            return new ResponseActionClose();
        }

        // TODO: Should check control digits.
        private bool IsValidAccountNumber(string accountNumber) => Regex.IsMatch(accountNumber, @"^\d{11}$");

        private string RemoveWhitespace(string s) => Regex.Replace(s, @"\s+", string.Empty);

        private async Task SubmitAccountDetails(Receival receival)
        {
            var approval = await _database.Approvals
                .Include(approval => approval.Review)
                    .ThenInclude(review => review.Grats)
                .SingleOrDefaultAsync(approval => approval.CorrelationId == receival.CorrelationId);

            if (approval == default)
            {
                _telemetry.TrackEntity($"{nameof(RegisterAccountDetails)}: Approval not found", receival);
                return;
            }

            receival.Approval = approval;
            receival.TeamId = approval.TeamId;

            var notifyReviewer = _components.ReviewGrats.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyReviewer);

            var notifyAuthor = _components.NotifyGratsSent.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyAuthor);

            var notifyReceiver = await _components.GratsReceived.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyReceiver);
        }
    }
}
