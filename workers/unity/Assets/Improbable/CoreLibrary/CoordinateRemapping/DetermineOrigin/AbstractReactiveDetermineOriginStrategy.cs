using System.Collections;
using Improbable.Math;
using log4net;
using UnityEngine;

namespace Improbable.CoreLibrary.CoordinateRemapping.DetermineOrigin
{
    /// <summary>
    /// This is a reactive version of a DetermineOriginStrategy. It triggers when it detects a remap for an
    /// entity which is beyond a certain distance from the origin. It also has a cooldown period between
    /// remaps.
    /// 
    /// When a remap is triggered, the DoOriginRecalcuation method is called at the end of the frame.
    /// 
    /// <seealso cref="AbstractDetermineOriginStrategy"/>
    /// </summary>
    public abstract class AbstractReactiveDetermineOriginStrategy : AbstractDetermineOriginStrategy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AbstractReactiveDetermineOriginStrategy));

        [Header("Reactive Trigger Settings")]
        [Tooltip("The cooldown between origin mapping.")]
        public float CooldownDurationBetweenOriginMovesInSeconds = 10.0f;
        [Tooltip("The distance from the origin which should trigger an origin recalculation.")]
        public float DistanceFromOriginToTriggerOriginCalculation = 50000.0f;

        private bool hasBeenInitialized;
        private float lastOriginMove;

        protected abstract void DoOriginRecalcuation();

        public override Vector3 GlobalPositionToUnityPosition(Vector3d globalPosition)
        {
            InitializeOriginIfNecessary(globalPosition);

            var unityPosition = base.GlobalPositionToUnityPosition(globalPosition);
            QueueRecalculationIfNecessary(unityPosition);
            return unityPosition;
        }

        public override Vector3d UnityPositionToGlobalPosition(Vector3 unityPosition)
        {
            QueueRecalculationIfNecessary(unityPosition);
            return base.UnityPositionToGlobalPosition(unityPosition);
        }

        private void InitializeOriginIfNecessary(Vector3d position)
        {
            if (!hasBeenInitialized)
            {
                Logger.InfoFormat("Setting the initial origin to {0}.", position);
                OffsetOrigin = position;
                hasBeenInitialized = true;
                lastOriginMove = Time.time;
            }
        }

        private void QueueRecalculationIfNecessary(Vector3 unityPosition)
        {
            if (ShouldOriginMove(unityPosition))
            {
                QueueOriginRecalculation();
            }
        }

        private bool ShouldOriginMove(Vector3 unityPosition)
        {
            return (Mathf.Abs(unityPosition.x) > DistanceFromOriginToTriggerOriginCalculation ||
                    Mathf.Abs(unityPosition.y) > DistanceFromOriginToTriggerOriginCalculation ||
                    Mathf.Abs(unityPosition.z) > DistanceFromOriginToTriggerOriginCalculation);
        }

        private void QueueOriginRecalculation()
        {
            StopAllCoroutines();
            StartCoroutine(OriginRecalculationWhenReady());
        }

        private IEnumerator OriginRecalculationWhenReady()
        {
            do
            {
                yield return new WaitForEndOfFrame();
            } while (Time.time < lastOriginMove + CooldownDurationBetweenOriginMovesInSeconds);

            DoOriginRecalcuation();
            lastOriginMove = Time.time;
        }
    }
}