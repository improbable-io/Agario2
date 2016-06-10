using Improbable.Util.Metrics;
using UnityEngine;

namespace Improbable.Unity.Util
{
    class MetricsReporter : MonoBehaviour
    {
        private IMetricsPublisher metricsPublisher;

        public void SetupDependencies(IMetricsPublisher publisher)
        {
            metricsPublisher = publisher;
        }

        public void Update()
        {
            if (metricsPublisher != null)
            {
                metricsPublisher.PublishScheduledMetrics();
            }
        }
    }
}