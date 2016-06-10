﻿using System.Diagnostics;
﻿using Improbable.Fapi.Protocol;
using Improbable.Unity.Core;
using Improbable.Util.Metrics;
using IoC;
using UnityEngine;

namespace Improbable.Metrics
{
    /// <summary>
    /// Takes the load reported by LoadMetricProvider specified by the EngineConfiguration
    /// or uses a default one if not specified, and reports it.
    /// </summary>
    class LoadReporter : MonoBehaviour
    {
        // Only query the load metric one per UpdatePeriodMs milliseconds
        public long UpdatePeriodMs = 2000;

        private readonly Stopwatch watch = new Stopwatch();
        [Inject] private IMetricsCollector metricsCollector;
        private long lastReported;

        private void Start()
        {
            watch.Start();
        }

        private void Update()
        {
            if (watch.ElapsedMilliseconds - lastReported > UpdatePeriodMs)
            {
                lastReported = watch.ElapsedMilliseconds;
                var loadProvider = EngineConfiguration.Instance.LoadMetricProvider;
                if (loadProvider == null)
                {
                    loadProvider = gameObject.GetComponent<UnityFixedFrameLoadMetricProvider>() ?? gameObject.AddComponent<UnityFixedFrameLoadMetricProvider>();
                    EngineConfiguration.Instance.LoadMetricProvider = loadProvider;
                }
                metricsCollector.Gauge(WorkerMetrics.LoadMetricKey).Set(loadProvider.GetLoad());
            }

        }
    }
}
