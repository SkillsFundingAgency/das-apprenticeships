using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace SFA.DAS.Apprenticeships.InnerApi.Telemetry
{
    /// <summary>
    /// Telemetry Processor that prevents 404 Not Found responses being logged as errors
    /// </summary>
    /// <param name="next"></param>
    public class NotFoundTelemetryProcessor(ITelemetryProcessor next) : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; } = next;

        /// <summary>
        /// Process a collected telemetry item.
        /// </summary>
        public void Process(ITelemetry item)
        {
            if (item is RequestTelemetry { ResponseCode: "404" } request)
            {
                request.Success = true;
            }

            Next.Process(item);
        }
    }
}
