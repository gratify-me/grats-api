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
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Action = Slack.Client.Interactions.Action;

namespace Gratify.Api.Components.Messages
{
    public class ReceiveGrats
    {
        private readonly string _transferToAccount = $"{typeof(ReceiveGrats)}.Yes";
        private readonly string _changeAccountDetails = $"{typeof(ReceiveGrats)}.ChangeAccountDetails";
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public ReceiveGrats(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
        }

        public async Task<PostMessage> Message(Approval approval)
        {
            var baseBlocks = await BaseBlocks(approval);

            return new PostMessage(
                text: Congratulations(approval),
                blocks: baseBlocks
                    .Append(new Actions(
                        id: "TransferOrChangeDetails",
                        elements: new Button[]
                        {
                            new Button(
                                id: _transferToAccount,
                                correlationId: approval.CorrelationId,
                                text: "Yes!",
                                style: ButtonStyle.Primary)
                        }))
                    .ToArray());
        }

        public async Task<UpdateMessage> UpdateReceived(Receival receival)
        {
            return await UpdateMessage(receival.Approval, new MrkdwnText[]
            {
                new MrkdwnText($"Account number registered! Working on transfering you the money :money_with_wings:")
            });
        }

        public async Task OnSubmit(Action action, string triggerId, string userId)
        {
            var modal = await _components.RegisterAccountDetails.Modal(action.CorrelationId, userId);
            await _slackService.OpenModal(triggerId, modal);
        }

        private string Congratulations(Approval approval) => $"Congratulations! <@{approval.Review.Grats.Author}> just sent you grats 🎉";

        private async Task<UpdateMessage> UpdateMessage(Approval approval, MrkdwnText[] updates)
        {
            var baseBlocks = await BaseBlocks(approval);

            return new UpdateMessage(
                text: Congratulations(approval),
                blocks: baseBlocks
                    .Append(new Context(updates))
                    .ToArray(),
                originalMessage: approval.ReceiverNotification);
        }

        private async Task<List<LayoutBlock>> BaseBlocks(Approval approval)
        {
            var settings = await _database.Settings.SingleOrDefaultAsync(setting => setting.TeamId == approval.TeamId);
            var amount = $"kr {settings.AmountPerGrats};-";
            if (settings == default)
            {
                _telemetry.TrackCorrelationId($"{nameof(ReceiveGrats)}: Settings not found", approval.CorrelationId);
                amount = "some money";
            }

            return new List<LayoutBlock>
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
                    text: $"Would you like {amount} to be transferred to your bank account?"),

                new Divider(),
            };
        }
    }
}
