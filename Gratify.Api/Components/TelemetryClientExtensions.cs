using System;
using System.Collections.Generic;
using Gratify.Api.Database.Entities;
using Microsoft.ApplicationInsights;

namespace Gratify.Api.Components
{
    public static class TelemetryClientExtensions
    {
        public static void TrackEntity(this TelemetryClient telemetry, string eventName, Entity entity) =>
            telemetry.TrackCorrelationId(eventName, entity.CorrelationId);

        public static void TrackCorrelationId(this TelemetryClient telemetry, string eventName, Guid correlationId) =>
            telemetry.TrackEvent(eventName, new Dictionary<string, string>
            {
                { "CorrelationId", correlationId.ToString() }
            });

        public static void TrackUserId(this TelemetryClient telemetry, string eventName, string userId) =>
            telemetry.TrackEvent(eventName, new Dictionary<string, string>
            {
                { "UserId", userId }
            });
    }
}
