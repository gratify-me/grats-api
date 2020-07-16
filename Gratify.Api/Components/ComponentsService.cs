using Gratify.Api.Components.HomeTabs;
using Gratify.Api.Components.Messages;
using Gratify.Api.Components.Modals;
using Gratify.Api.Database;
using Microsoft.ApplicationInsights;
using Slack.Client;

namespace Gratify.Api.Components
{
    public class ComponentsService
    {
        public GratsReceived GratsReceived { get; }

        public NotifyGratsSent NotifyGratsSent { get; }

        public RequestGratsReview RequestGratsReview { get; }

        public DenyGrats DenyGrats { get; }

        public ForwardGrats ForwardGrats { get; }

        public SendGrats SendGrats { get; }

        public AllGratsSpent AllGratsSpent { get; }

        public ShowAppHome ShowAppHome { get; }

        public AddTeamMember AddTeamMember { get; }

        public ChangeSettings ChangeSettings { get; }

        public ComponentsService(TelemetryClient telemetry, SlackService slackService, GratsDb database)
        {
            GratsReceived = new GratsReceived(telemetry, database, slackService, this);
            NotifyGratsSent = new NotifyGratsSent(telemetry, slackService);
            RequestGratsReview = new RequestGratsReview(telemetry, database, slackService, this);
            DenyGrats = new DenyGrats(telemetry, database, slackService, this);
            ForwardGrats = new ForwardGrats(telemetry, database, slackService, this);
            SendGrats = new SendGrats(telemetry, database, slackService, this);
            AllGratsSpent = new AllGratsSpent(telemetry, database, this);
            ShowAppHome = new ShowAppHome(telemetry, database, slackService, this);
            AddTeamMember = new AddTeamMember(telemetry, database, slackService, this);
            ChangeSettings = new ChangeSettings(telemetry, database, slackService, this);
        }
    }
}
