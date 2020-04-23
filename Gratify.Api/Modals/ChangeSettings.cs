using System;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Services;
using Slack.Client.BlockKit.BlockElements.Selects;
using Slack.Client.BlockKit.CompositionObjects;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Interactions;
using Slack.Client.Views;

namespace Gratify.Api.Modals
{
    public class ChangeSettings
    {
        private readonly InteractionService _interactions;

        public ChangeSettings(InteractionService interactions)
        {
            _interactions = interactions;
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
                });

        public async Task<ResponseAction> OnSubmit(ViewSubmission submission)
        {
            var gratsPeriodInDays = submission.GetStateValue<StaticSelect>("SelectGratsPeriod.GratsPeriodInDays");
            var numberOfGratsPerPeriod = submission.GetStateValue<StaticSelect>("SelectNumberOfGrats.NumberOfGratsPerPeriod");

            await _interactions.ChangeSettings(
                teamId: submission.Team.Id,
                userId: submission.User.Id,
                gratsPeriodInDays: int.Parse(gratsPeriodInDays.SelectedOption.Value),
                numberOfGratsPerPeriod: int.Parse(numberOfGratsPerPeriod.SelectedOption.Value));

            return new ResponseActionClose();
        }
    }
}
