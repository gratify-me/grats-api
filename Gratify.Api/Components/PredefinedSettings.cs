using System.Linq;
using Gratify.Api.Database.Entities;

namespace Gratify.Api.Components
{
    public class PredefinedSetting
    {
        public static readonly PredefinedSetting[] AllPredefinedSettings = new PredefinedSetting[]
        {
            new PredefinedSetting(canSendNoOfGrats: 1, everyNoOfDays: 0, eachWorthNoOfNok: 1, meaning: "Everyone can send an unlimited amount of Grats worth kr 1;-"),
            new PredefinedSetting(canSendNoOfGrats: 2, everyNoOfDays: 7, eachWorthNoOfNok: 50),
            new PredefinedSetting(canSendNoOfGrats: 1, everyNoOfDays: 30, eachWorthNoOfNok: 1500),
            new PredefinedSetting(canSendNoOfGrats: 0, everyNoOfDays: 1, eachWorthNoOfNok: 0, meaning: "Sending Grats is disabled"),
        };

        public static PredefinedSetting From(Settings settings)
        {
            var predefinedSetting = AllPredefinedSettings.First(setting =>
                setting.GratsPeriodInDays == settings.GratsPeriodInDays &&
                setting.NumberOfGratsPerPeriod == settings.NumberOfGratsPerPeriod &&
                setting.AmountPerGrats == settings.AmountPerGrats);

            if (predefinedSetting != default)
            {
                return predefinedSetting;
            }

            return new PredefinedSetting(settings.NumberOfGratsPerPeriod, settings.GratsPeriodInDays, settings.AmountPerGrats);
        }

        public static PredefinedSetting From(string description) => AllPredefinedSettings.First(setting => setting.Description() == description);

        public int GratsPeriodInDays { get; }

        public int NumberOfGratsPerPeriod { get; }

        public int AmountPerGrats { get; }

        public string Meaning { get; }

        public PredefinedSetting(int canSendNoOfGrats, int everyNoOfDays, int eachWorthNoOfNok, string meaning = "")
        {
            NumberOfGratsPerPeriod = canSendNoOfGrats;
            GratsPeriodInDays = everyNoOfDays;
            AmountPerGrats = eachWorthNoOfNok;
            Meaning = meaning;
        }

        public string Description()
        {
            if (!string.IsNullOrEmpty(Meaning))
            {
                return Meaning;
            }

            return $"Everyone can send {NumberOfGratsPerPeriod} Grats every {GratsPeriod} worth kr {AmountPerGrats};-";
        }

        private string GratsPeriod => GratsPeriodInDays switch
        {
            1 => "day",
            7 => "week",
            _ => $"{GratsPeriodInDays} days",
        };
    }
}
