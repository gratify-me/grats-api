using System;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Slack.Client;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Components.Modals
{
    public class ChangeSettings
    {
        private readonly TelemetryClient _telemetry;
        private readonly GratsDb _database;
        private readonly SlackService _slackService;
        private readonly ComponentsService _components;

        public ChangeSettings(TelemetryClient telemetry, GratsDb database, SlackService slackService, ComponentsService components)
        {
            _telemetry = telemetry;
            _database = database;
            _slackService = slackService;
            _components = components;
        }

        public Modal Modal(Settings settings) =>
            new Modal(
                id: typeof(ChangeSettings),
                correlationId: Guid.NewGuid(),
                title: "Change Settings",
                submit: "Save",
                close: "Cancel",
                blocks: new LayoutBlock[]
                {
                    new Input(
                        id: "SelectGratsPeriod",
                        label: ":calendar: Grats period in days",
                        element: new StaticSelect(
                            id: "GratsPeriodInDays",
                            initialOption: new Option(settings.GratsPeriodInDays.ToString()),
                            options: Enumerable
                                .Range(0, 31)
                                .Select(n => new Option(n.ToString()))
                                .ToArray())),

                    new Section(
                        id: "SelectGratsPeriodInfo",
                        text: "Use *0* if you don't wish to use a Grats period"),

                    new Input(
                        id: "SelectNumberOfGrats",
                        label: ":grats: Number of Grats per period",
                        element: new StaticSelect(
                            id: "NumberOfGratsPerPeriod",
                            initialOption: new Option(settings.NumberOfGratsPerPeriod.ToString()),
                            options: Enumerable
                                .Range(1, 11)
                                .Select(n => new Option(n.ToString()))
                                .ToArray())),

                    new Section(
                        id: "SelectNumberOfGratsInfo",
                        text: "The number of Grats a user can send every period"),

                    new Input(
                        id: "SelectAmountPerGrats",
                        label: ":moneybag: Amount of money per Grats",
                        element: new StaticSelect(
                            id: "AmountPerGrats",
                            initialOption: new Option(settings.AmountPerGrats.ToString()),
                            options: new Option[]
                            {
                                new Option("0"),
                                new Option("50"),
                                new Option("1500"),
                                new Option("3000"),
                            })),

                    new Section(
                        id: "SelectAmountPerGratsInfo",
                        text: "The amount of money a user receives as part of the Grats"),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var gratsPeriodInDays = submission.GetStateValue<StaticSelect>("SelectGratsPeriod.GratsPeriodInDays");
            var numberOfGratsPerPeriod = submission.GetStateValue<StaticSelect>("SelectNumberOfGrats.NumberOfGratsPerPeriod");
            var amountPerGrats = submission.GetStateValue<StaticSelect>("SelectAmountPerGrats.AmountPerGrats");

            await SaveNewSettings(
                teamId: submission.Team.Id,
                userId: submission.User.Id,
                gratsPeriodInDays: int.Parse(gratsPeriodInDays.SelectedOption.Value),
                numberOfGratsPerPeriod: int.Parse(numberOfGratsPerPeriod.SelectedOption.Value),
                amountPerGrats: int.Parse(amountPerGrats.SelectedOption.Value));

            return new ResponseActionClose();
        }

        public async Task SaveNewSettings(string teamId, string userId, int gratsPeriodInDays, int numberOfGratsPerPeriod, int amountPerGrats)
        {
            var settings = await _database.Settings.SingleOrDefaultAsync(setting => setting.TeamId == teamId);
            if (settings == default)
            {
                _telemetry.TrackUserId($"{nameof(ChangeSettings)}: Settings not found", userId);
                return;
            }

            settings.GratsPeriodInDays = gratsPeriodInDays;
            settings.NumberOfGratsPerPeriod = numberOfGratsPerPeriod;
            settings.AmountPerGrats = amountPerGrats;
            settings.UpdatedAt = DateTime.UtcNow;
            settings.UpdatedBy = userId;

            await _database.SaveChangesAsync();
            var homeBlocks = await _components.ShowAppHome.HomeTab(teamId, userId);
            await _slackService.PublishModal(userId, homeBlocks);
        }
    }
}
