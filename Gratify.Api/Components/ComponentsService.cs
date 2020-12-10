using Gratify.Api.Components.HomeTabs;
using Gratify.Api.Components.Messages;
using Gratify.Api.Components.Modals;
using Gratify.Api.Database;
using Gratify.Api.Services;
using Microsoft.ApplicationInsights;
using Slack.Client;

namespace Gratify.Api.Components
{
    public class ComponentsService
    {
        public AllGratsSpent AllGratsSpent { get; }

        public GratsRemaining GratsRemaining { get; }

        public SendGrats SendGrats { get; }

        public GratsSent NotifyGratsSent { get; }

        public ReviewGrats ReviewGrats { get; }

        public DenyGrats DenyGrats { get; }

        public ForwardReview ForwardGrats { get; }

        public ReceiveGrats GratsReceived { get; }

        public RegisterAccountDetails RegisterAccountDetails { get; }

        public ShowAppHome ShowAppHome { get; }

        public AddTeamMember AddTeamMember { get; }

        public ChangeSettings ChangeSettings { get; }

        public SendFeedback SendFeedback { get; }

        public ComponentsService(TelemetryClient telemetry, SlackService slackService, GratsDb database, EmailClient emailClient)
        {
            AllGratsSpent = new AllGratsSpent(telemetry, database, this);
            GratsRemaining = new GratsRemaining(telemetry, slackService, this);
            SendGrats = new SendGrats(telemetry, database, slackService, this);
            NotifyGratsSent = new GratsSent(telemetry, slackService);
            ReviewGrats = new ReviewGrats(telemetry, database, slackService, this);
            DenyGrats = new DenyGrats(telemetry, database, slackService, this);
            ForwardGrats = new ForwardReview(telemetry, database, slackService, this);
            GratsReceived = new ReceiveGrats(telemetry, database, slackService, this);
            RegisterAccountDetails = new RegisterAccountDetails(telemetry, database, slackService, this);
            ShowAppHome = new ShowAppHome(telemetry, database, slackService, this);
            AddTeamMember = new AddTeamMember(telemetry, database, slackService, this);
            ChangeSettings = new ChangeSettings(telemetry, database, slackService, this);
            SendFeedback = new SendFeedback(telemetry, slackService, emailClient);
        }
    }
}
