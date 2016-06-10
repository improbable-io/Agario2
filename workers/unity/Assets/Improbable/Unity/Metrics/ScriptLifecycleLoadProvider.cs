using UnityEngine;

namespace Improbable.Metrics
{
    /// <summary>
    /// Provides an estimation of the load of the Unity worker based on analysis of
    /// the Unity Script Lifecycle http://docs.unity3d.com/Manual/ExecutionOrder.html
    /// Uses targetFrameRate and target fixed frame rate (Time.fixedDeltaTime / Time.timeScale)
    /// to estimate load.
    /// </summary>
    class ScriptLifecycleLoadProvider : ILoadMetricProvider
    {
        private readonly ScriptLifecycleAnalytics scriptLifecycleAnalytics;
        private long lastPhysicsLoopCount;
        private long lastRenderLoopCount;
        private float lastPhysicsLoopTime;
        private float lastRenderLoopTime;

        private float timePerPhysics;
        private float timePerRender;

        public ScriptLifecycleLoadProvider(ScriptLifecycleAnalytics scriptLifecycleAnalytics)
        {
            this.scriptLifecycleAnalytics = scriptLifecycleAnalytics;
        }

        /// <returns>
        /// A load estimation from the last time this method was called.
        /// If called more frequently, will return noisier data.
        /// If called before more analytics is present, will return the last recorded load.
        /// </returns>
        public float GetLoad()
        {
            UpdateTimePerPhysicsLoop();
            UpdateTimePerRenderLoop();

            var targetRenderCount = Application.targetFrameRate;
            var targetPhysicsTime = Time.fixedDeltaTime / Time.timeScale;

            return timePerPhysics / targetPhysicsTime + timePerRender * targetRenderCount;
        }

        private void UpdateTimePerPhysicsLoop()
        {
            var newPhysicsLoopCount = scriptLifecycleAnalytics.FixedFrameCount;
            if (newPhysicsLoopCount - lastPhysicsLoopCount > 0)
            {
                var newPhysicsLoopTime = scriptLifecycleAnalytics.CumulativePhysicsLoopDuration();
                timePerPhysics = (newPhysicsLoopTime - lastPhysicsLoopTime) / (newPhysicsLoopCount - lastPhysicsLoopCount);

                lastPhysicsLoopTime = newPhysicsLoopTime;
                lastPhysicsLoopCount = newPhysicsLoopCount;
            }
        }

        private void UpdateTimePerRenderLoop()
        {
            var newRenderLoopCount = scriptLifecycleAnalytics.FrameCount;
            if (newRenderLoopCount - lastRenderLoopCount > 0)
            {
                var newRenderLoopTime = scriptLifecycleAnalytics.CumulativeRenderLoopDuration();
                timePerRender = (newRenderLoopTime - lastRenderLoopTime) / (newRenderLoopCount - lastRenderLoopCount);

                lastRenderLoopTime = newRenderLoopTime;
                lastRenderLoopCount = newRenderLoopCount;
            }
        }
    }
}
