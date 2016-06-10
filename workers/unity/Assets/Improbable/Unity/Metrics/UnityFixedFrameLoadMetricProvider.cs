using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Improbable.Metrics
{
    class UnityFixedFrameLoadMetricProvider : MonoBehaviour, ILoadMetricProvider
    {
        private const long UpdateMinPeriodMillis = 2000; // 1 update per 2 seconds.
        private static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        private readonly Stopwatch stopwatch = new Stopwatch();

        private float load;
        private TimeSpan lastFixedUpdateStart;
        private TimeSpan lastUpdatedLoadMetric;
        private TimeSpan cumulativeTimeTakenForFixedUpdate;

        public void Start()
        {
            stopwatch.Start();
        }

        public float GetLoad()
        {
            return load;
        }

        public void FixedUpdate()
        {
            lastFixedUpdateStart = stopwatch.Elapsed;
            if (FrameRendered(stopwatch.ElapsedMilliseconds))
            {
                var sinceLoadLastUpdated = lastFixedUpdateStart - lastUpdatedLoadMetric;
                load = (float)(1.0 * cumulativeTimeTakenForFixedUpdate.Ticks / sinceLoadLastUpdated.Ticks);
                cumulativeTimeTakenForFixedUpdate = TimeSpan.Zero;
                lastUpdatedLoadMetric = lastFixedUpdateStart;
            }
            StartCoroutine(EndOfFrame());
        }

        private IEnumerator EndOfFrame()
        {
            yield return WaitForEndOfFrame;
            cumulativeTimeTakenForFixedUpdate += stopwatch.Elapsed - lastFixedUpdateStart;
        }

        private long startedAt;
        /* Time.deltaTime gives you the time to render the last frame.
         * dt is then the sum of frame rendering time since last fps update.
         * frameCount is the number of frame renderings since last fps update.
        */
        private bool FrameRendered(long currentTime)
        {
            var elapsed = currentTime - startedAt;
            if (elapsed > UpdateMinPeriodMillis)
            {
                startedAt = currentTime;
                return true;
            }
            return false;
        }
    }
}
