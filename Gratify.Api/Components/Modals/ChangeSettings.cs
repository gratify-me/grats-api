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
                        id: "SelectHowUsersCanSendGrats",
                        label: ":question: How can users send Grats?",
                        element: new StaticSelect(
                            id: "UsersCanSendGrats",
                            initialOption: new Option(PredefinedSetting.From(settings).Description()),
                            options: PredefinedSetting.AllPredefinedSettings
                                .Select(setting => new Option(setting.Description()))
                                .ToArray())),
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var usersCanSendGrats = submission.GetStateValue<StaticSelect>("SelectHowUsersCanSendGrats.UsersCanSendGrats");
            var predefinedSetting = PredefinedSetting.From(usersCanSendGrats.SelectedOption.Value);

            await SaveNewSettings(
                teamId: submission.Team.Id,
                userId: submission.User.Id,
                gratsPeriodInDays: predefinedSetting.GratsPeriodInDays,
                numberOfGratsPerPeriod: predefinedSetting.NumberOfGratsPerPeriod,
                amountPerGrats: predefinedSetting.AmountPerGrats);

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
