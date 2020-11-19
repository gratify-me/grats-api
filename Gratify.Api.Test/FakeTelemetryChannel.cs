using System;
using Microsoft.ApplicationInsights.Channel;

namespace Gratify.Api.Test
{
    public class FakeTelemetryChannel : ITelemetryChannel
    {
        public bool? DeveloperMode { get; set; }

        public string EndpointAddress { get; set; }

        public void Dispose()
        { }

        public void Flush()
        { }

        public void Send(ITelemetry item)
        {
            Console.WriteLine(item.ToString());
        }
    }
}
