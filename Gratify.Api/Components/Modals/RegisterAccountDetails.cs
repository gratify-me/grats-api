using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.CompositionObjects;
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

        public async Task<Modal> Modal(Guid correlationId, string userId)
        {
            var user = await _database.Users.SingleAsync(user => user.UserId == userId);

            return new Modal(
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
                            initialValue: user.AccountNumber,
                            multiline: false)),

                    new Input(
                        id: "InputSaveAccountNumber",
                        label: "Save account number for future Grats?",
                        element: SelectSaveAccountNumber(user),
                        optional: true),
                });
        }

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var accountNumberInput = submission.GetStateValue<PlainTextInput>("InputReceiverAccountNumber.AccountNumber");
            var accountNumber = RemoveWhitespace(accountNumberInput.Value);
            if (!IsValidAccountNumber(accountNumber))
            {
                return new ResponseActionErrors("InputReceiverAccountNumber", $"{accountNumberInput.Value} is not a valid Norwegian account number");
            }

            var saveAccountNumberInput = submission.GetStateValue<CheckboxGroup>("InputSaveAccountNumber.SaveAccountNumber");
            var saveAccountNumber = saveAccountNumberInput?.SelectedOptions?.Any(option => option == Option.Yes);

            await SubmitAccountDetails(submission, accountNumber, saveAccountNumber.Value);

            return new ResponseActionClose();
        }

        // TODO: Should check control digits.
        private bool IsValidAccountNumber(string accountNumber) => Regex.IsMatch(accountNumber, @"^\d{11}$");

        private string RemoveWhitespace(string s) => Regex.Replace(s, @"\s+", string.Empty);

        private async Task SubmitAccountDetails(ViewSubmission submission, string accountNumber, bool saveAccountNumber = false)
        {
            var approval = await _database.Approvals
                .Include(approval => approval.Review)
                    .ThenInclude(review => review.Grats)
                .SingleOrDefaultAsync(approval => approval.CorrelationId == submission.CorrelationId);

            if (approval == default)
            {
                _telemetry.TrackCorrelationId($"{nameof(RegisterAccountDetails)}: Approval not found", submission.CorrelationId);
                return;
            }

            var user = await _database.Users.SingleAsync(user => user.UserId == submission.User.Id);
            user.AccountNumber = saveAccountNumber ? accountNumber : null;

            var settings = await _database.Settings.SingleAsync(setting => setting.TeamId == submission.Team.Id);
            var receival = new Receival(
                correlationId: submission.CorrelationId,
                receivedAt: DateTime.UtcNow,
                receiverName: submission.User.Id, // TODO: RealName is not available her. What can we do to amend this?
                receiverAccountNumber: accountNumber,
                amountReceived: settings.AmountPerGrats)
            {
                Approval = approval,
                TeamId = approval.TeamId,
            };

            await _database.Receivals.AddAsync(receival);
            await _database.SaveChangesAsync();

            var notifyReviewer = _components.ReviewGrats.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyReviewer);

            var notifyAuthor = _components.NotifyGratsSent.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyAuthor);

            var notifyReceiver = await _components.GratsReceived.UpdateReceived(receival);
            await _slackService.UpdateMessage(notifyReceiver);
        }

        private CheckboxGroup SelectSaveAccountNumber(User user)
        {
            if (user.AccountNumber != null)
            {
                return new CheckboxGroup(
                    id: "SaveAccountNumber",
                    options: new Option[] { Option.Yes },
                    initialOption: Option.Yes);
            }

            return new CheckboxGroup(
                id: "SaveAccountNumber",
                options: new Option[] { Option.Yes });
        }
    }
}
