using Assets.Improbable.Gel.Util.Metrics;
using Improbable.Corelib.Metrics;
using Improbable.Unity.Visualizer;
using Improbable.Util.Metrics;
using IoC;
using UnityEngine;

namespace Improbable.Corelib.LagDetection
{
    public class GSimLatency : MonoBehaviour
    {
        private float lastTimeOfSentPing;

        [Require] protected EngineLatencyWriter lagDetectionState;
        [Require] protected EngineLatencyReplyReader lagResponseState;
        
        [Inject] public IMetricsCollector Collector { get; set; }

        private IMetricsUpdater metricsUpdater;
        private IHistogram histogramMetrics;

        protected void OnEnable()
        {
            metricsUpdater = MetricsUpdatersManager.GetUpdater("GSim Latency");
            histogramMetrics = Collector.Histogram("gsim_latency", new double[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048 });
            lagResponseState.EnginePingReceived += OnPingReceived;
        }

        protected void Update()
        {
            float currentTime = Time.time;
            if (IsTimeToRefreshLag(currentTime))
            {
                lagDetectionState.Update
                    .TriggerEnginePingSent(ToMillis(currentTime))
                    .FinishAndSend();
                lastTimeOfSentPing = currentTime;
            }
        }

        private void OnPingReceived(EnginePingReceived pingReceived)
        {
            int currentTimeInMillis = ToMillis(Time.time);
            var latency = currentTimeInMillis - pingReceived.ReceivedPingTimestampMillis;
            lagDetectionState.Update
                 .RoundTripMillis(latency)
                 .FinishAndSend();

            metricsUpdater.Update(latency);
            histogramMetrics.AddObservation(latency);
        }

        private bool IsTimeToRefreshLag(float currentTime)
        {
            return currentTime - lastTimeOfSentPing > (lagDetectionState.RefreshPeriodMillis / 1000f);
        }

        private static int ToMillis(float currentTime)
        {
            return (int)System.Math.Floor(currentTime * 1000);
        }
    }
}
