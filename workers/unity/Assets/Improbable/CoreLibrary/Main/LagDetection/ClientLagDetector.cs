using Assets.Improbable.Gel.Util.Metrics;
using Improbable.Corelib.Metrics;
using Improbable.Unity.Visualizer;
using Improbable.Util.Metrics;
using UnityEngine;
using IoC;

namespace Improbable.Corelib.LagDetection
{
    public class ClientLagDetector : MonoBehaviour
    {
        private float lastTimeOfSentPing;
        private IMetricsUpdater metricsUpdater;
        private IHistogram histogramMetrics;

        [Require] protected ClientPhysicsLatencyReplyReader physicsLagReplierState;
        [Require] protected ClientPhysicsLatencyWriter lagDetectionState;

        [Inject] public IMetricsCollector Collector { get; set; }

        protected void OnEnable()
        {
            metricsUpdater = MetricsUpdatersManager.GetUpdater("RTT FSim Latency (ms)");
            histogramMetrics = Collector.Histogram("rtt_fsim_latency_ms", new double[] {1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048});
            physicsLagReplierState.ClientPhysicsPingReceived += OnPhysicsLagPingReceived;
        }

        protected void Update()
        {
            float currentTime = Time.time;
            if (IsTimeToRefreshLag(currentTime))
            {
                lagDetectionState.Update
                     .TriggerClientPhysicsPingSent(ToMillis(currentTime))
                     .FinishAndSend();
                lastTimeOfSentPing = currentTime;
            }
        }

        private void OnPhysicsLagPingReceived(ClientPhysicsPingReceived pingReceived)
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
