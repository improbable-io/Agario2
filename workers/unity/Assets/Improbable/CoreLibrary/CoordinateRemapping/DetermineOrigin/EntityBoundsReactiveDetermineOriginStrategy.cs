using Improbable.Core.Entity;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Entity;
using IoC;
using log4net;
using UnityEngine;

namespace Improbable.CoreLibrary.CoordinateRemapping.DetermineOrigin
{
    /// <summary>
    /// The EntityBoundsReactiveDetermineOriginStrategy is an implementation of the AbstractReactiveDetermineOriginStrategy
    /// which uses the EntityBounds method to determine the new Origin.
    /// 
    /// If the new origin is still within the trigger range, the method will also increase the threshold.
    /// 
    /// The EntityBounds method only uses EntityObjects to determine the Bounds.
    /// </summary>
    public class EntityBoundsReactiveDetermineOriginStrategy : AbstractReactiveDetermineOriginStrategy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EntityBoundsReactiveDetermineOriginStrategy));

        [Inject] protected IUniverse Universe;

        [Header("Entity Bounds Settings")]
        [Tooltip("The difference between origin threshold which allows remap to take place.")]
        public float OriginSqureDistanceThreshold = 100f * 100f;
        [Tooltip("The amount to increase the threshold by if after remapping there is still an entity which will trigger " +
                 "the reactive origin remap.")]
        public float OnEntityOutOfBoundsIncreaseThresholdBy = 10000f;
        [Tooltip("Whether non-authoritative entities should be considered when calculating the bounds.")]
        public bool ConsiderNonAuthoritativeEntities = false;

        protected override void DoOriginRecalcuation()
        {
            if (Universe == null)
            {
                Logger.ErrorFormat("IUniverse was not injected properly into the {0}. Cannot recalculate Origin.", GetType().Name);
                return;
            }
            var entityBounds = EntityBounds.CalculateBoundsOfAllEntities(Universe, ShouldConsiderObject);

            var originOffset = entityBounds.MidVector();
            if (ApplyOriginOffset(originOffset))
            {
                entityBounds = entityBounds.OffsetBy(-originOffset);
            }

            IncreaseThresholdIfNecessary(entityBounds);
        }

        public virtual bool ShouldConsiderObject(IEntityObject entityObject)
        {
            return ConsiderNonAuthoritativeEntities || entityObject.HasAuthoritativeState();
        }

        private void IncreaseThresholdIfNecessary(EntityBounds entityBounds)
        {
            var maxAbsoluteValue = entityBounds.GetMaxAbsoluteAxisValue();
            if (maxAbsoluteValue >= DistanceFromOriginToTriggerOriginCalculation)
            {
                var newThresholdRange = maxAbsoluteValue + OnEntityOutOfBoundsIncreaseThresholdBy;

                Logger.WarnFormat("After remapping, an entity object has an axis value of {0} which is still outside the trigger range of {1}. " +
                                  "Increasing the trigger threshold range to {2}.",
                    maxAbsoluteValue, 
                    DistanceFromOriginToTriggerOriginCalculation,
                    newThresholdRange);

                DistanceFromOriginToTriggerOriginCalculation = newThresholdRange;
            }
        }

        protected virtual bool ApplyOriginOffset(Vector3 newOrigin)
        {
            if (!IsOriginOffsetPastThreshold(newOrigin))
            {
                return false;
            }

            OffsetOrigin += newOrigin.ToNativeVector();
            Logger.InfoFormat("Origin moved to {0}.", OffsetOrigin);

            return true;
        }

        protected virtual bool IsOriginOffsetPastThreshold(Vector3 newOrigin)
        {
            return newOrigin.sqrMagnitude > OriginSqureDistanceThreshold;
        }
    }
}
