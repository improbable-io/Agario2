using System.Diagnostics;
using Improbable.Util.Metrics;
using IoC;
using UnityEngine;

namespace Improbable.Unity.Util
{
    public class EngineMetricsFPS : MonoBehaviour
    {
        private const long UpdateMinPeriodMillis = 2000; // 1 update per 2 seconds.
        private readonly Stopwatch stopwatch = new Stopwatch();
        private FPSMetric dynamicRate;
        private FPSMetric fixedRate;

        [Inject] private IMetricsCollector MetricsCollector { get; set; }
       
        public void SetupDependencies()
        {
            dynamicRate = new FPSMetric("Dynamic", MetricsCollector);
            fixedRate = new FPSMetric("Fixed", MetricsCollector);
            stopwatch.Start();
        }

        public void FixedUpdate()
        {
            if (fixedRate != null)
            {
                fixedRate.FrameRendered(stopwatch.ElapsedMilliseconds);
            }
        }

        public void Update()
        {
            if (dynamicRate != null)
            {
                dynamicRate.FrameRendered(stopwatch.ElapsedMilliseconds);
            }
        }

        private class FPSMetric
        {
            private long startedAt;
            private int frameCount;
            private double dt;
            private double fps;

            private readonly IGauge fpsGauge;
            private readonly IGauge artGauge;

            internal FPSMetric(string prefix, IMetricsCollector metricsCollector)
            {
                fpsGauge = metricsCollector.Gauge(prefix + ".FPS");
                artGauge = metricsCollector.Gauge(prefix + ".AverageRenderTime");
            }

            /* Time.deltaTime gives you the time to render the last frame.
             * dt is then the sum of frame rendering time since last fps update.
             * frameCount is the number of frame renderings since last fps update.
             */
            internal void FrameRendered(long currentTime)
            {
                frameCount++;
                dt += (1000 * Time.deltaTime);
                var elapsed = currentTime - startedAt;
                if (elapsed > UpdateMinPeriodMillis)
                {
                    fps = ((frameCount * 1000.0) / elapsed);
                    WriteMetrics();
                    frameCount = 0;
                    dt = 0;
                    startedAt = currentTime;
                }
            }

            private void WriteMetrics()
            {
                fpsGauge.Set((float)fps);
                artGauge.Set((float)(dt / frameCount));
            }
        }
    }
}
