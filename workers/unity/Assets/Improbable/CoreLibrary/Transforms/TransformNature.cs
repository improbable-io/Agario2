using System;
using UnityEngine;
using System.Collections.Generic;
using Improbable.Corelib.Physical;
using Improbable.Corelib.PrefabExporting.PreProcessors;
using Improbable.CoreLibrary.Transforms.Global;
using Improbable.CoreLibrary.Transforms.Hierarchy;
using Improbable.CoreLibrary.Transforms.Local;
using Improbable.CoreLibrary.Transforms.Offsets;
using Improbable.Unity.Export;
using log4net;

namespace Improbable.CoreLibrary.Transforms
{
    /// <summary>
    /// The TransformNature adds the Unity Behaviours for the TransformNature. For every engine, it provides two top-level
    /// choices: A Non-Authoritative Mode and whether the Authoritative MonoBehaviours should be added to the object.
    /// 
    /// There also a variety of settings for the Authoritative mode and the Interpolation Non-Authoritative mode. 
    /// </summary>
    [KeepOnExportedPrefab]
    public class TransformNature : PreProcessorBase, ILerpTransformSettings, IUpdateTransformSettings
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TransformNature));

        [Header("Client Settings")] 
        [Tooltip("How the client should act when in non-authoritative mode.")]
        public NonAuthoritativeMode ClientNonAuthoritativeMode = NonAuthoritativeMode.Lerp;
        [Tooltip("Whether it is possible for the client to be Authoritative.")] 
        public bool ClientCanBeAuthoritative = true;
        [Tooltip("Whether to fallback to global mode if the entity's parent is not in the scene.")]
        public bool ClientEnableGlobalFallback = true;

        [Header("FSim Settings")]
        [Tooltip("How the fsim should act when in non-authoritative mode.")]
        public NonAuthoritativeMode FSimNonAuthoritativeMode = NonAuthoritativeMode.Exact;
        [Tooltip("Whether it is possible for the fsim to be Authoritative.")]
        public bool FSimCanBeAuthoritative = true;
        [Tooltip("Whether to fallback to global mode if the entity's parent is not in the scene.")]
        public bool FSimEnableGlobalFallback = true;

        [Header("State Update Configuration")]
        [Tooltip("Whether to use the RigidBody center of mass as the base pivot location.")]
        public bool UseCenterOfMassPivot;
        [Tooltip("Specify a custom pivot location")]
        public Vector3 PivotOffset;
        [Tooltip("The minimum number of seconds between sending each Transform state update.")]
        public float NetworkUpdatePeriodThreshold = DefaultPhysicalParameters.NetworkUpdatePeriodThreshold;
        [Tooltip("The minimum square distance from the last sent position before sending a new Transform state update.")]
        public float PositionNetworkUpdateSquareDistanceThreshold = DefaultPhysicalParameters.PositionNetworkUpdateSquareDistanceThreshold;
        [Tooltip("The minimum angle in degrees from the last sent rotation before sending a new Transform state update.")]
        public float RotationNetworkUpdateAngleThreshold = DefaultPhysicalParameters.RotationNetworkUpdateAngleThreshold;

        [Header("Interpolation Configuration")]
        [Tooltip("The number of seconds that the interpolator should lerp")]
        public float InterpolationDelayInSeconds = 0.2f;
        [Tooltip("The minimum angle in degrees from the last interpolated rotation to the target interpolated rotation before setting the rotation to the target.")]
        public float MinAngleToInterpolateBetween = DefaultPhysicalParameters.MinAngleToInterpolateBetween;
        [Tooltip("The minimum square distance from the last interpolated position to the target interpolated position before setting the position to the target.")]
        public float MinDistanceToInterpolateBetween = DefaultPhysicalParameters.MinDistanceToInterpolateBetween;
        [Tooltip("The number of seconds after receiving a Transform state update that we will continue to interpolate before ceasing interpolation.")]
        public float MaxSecondsToInterpolateAfterLastUpdate = DefaultPhysicalParameters.MaxSecondsToInterpolateAfterLastUpdate;

        [Header("Optimization Settings")]
        [Tooltip("Whether the GameObject can be parented. Performance optimizations can be made if false.")]
        public bool GameObjectCanBeParented = true;
        [Tooltip("Whether the GameObject can be a parent. Performance optimizations can be made if false.")]
        public bool GameObjectCanBeParent = true;

        public enum NonAuthoritativeMode
        {
            Lerp, Exact, Static, Custom
        }

        protected override Dictionary<Type, VisualizerPreProcessorConfig> GetCommonVisualizersToAdd()
        {
            return new Dictionary<Type, VisualizerPreProcessorConfig>
            {
                { typeof (TransformOffsetsRegistry), new VisualizerPreProcessorConfig(ShouldAddParentBehaviours, AddTransformOffsetRegistry) },
                { typeof (TransformParentHierarchyBehaviour), new VisualizerPreProcessorConfig(ShouldAddParentBehaviours) },
                { typeof (TransformChildHierarchyBehaviour), new VisualizerPreProcessorConfig(ShouldAddParentedBehaviours) },
                { typeof (TransformNonHierarchyBehaviour), new VisualizerPreProcessorConfig(ShouldAddNonParentedBehaviours) }
            };
        }

        protected override Dictionary<Type, VisualizerPreProcessorConfig> GetClientVisualizersToAdd()
        {
            return new Dictionary<Type, VisualizerPreProcessorConfig>
            {
                { typeof (ExactGlobalTransformBehaviour), new VisualizerPreProcessorConfig(IsClientNonAuthoritativeMode(NonAuthoritativeMode.Exact, true)) },
                { typeof (ExactLocalTransformBehaviour), new VisualizerPreProcessorConfig(IsClientNonAuthoritativeMode(NonAuthoritativeMode.Exact)) },
                { typeof (LerpGlobalTransformBehaviour), new VisualizerPreProcessorConfig(IsClientNonAuthoritativeMode(NonAuthoritativeMode.Lerp, true)) },
                { typeof (LerpLocalTransformBehaviour), new VisualizerPreProcessorConfig(IsClientNonAuthoritativeMode(NonAuthoritativeMode.Lerp)) },
                { typeof (StaticGlobalTransformBehaviour), new VisualizerPreProcessorConfig(IsClientNonAuthoritativeMode(NonAuthoritativeMode.Static, true)) },
                { typeof (StaticLocalTransformBehaviour), new VisualizerPreProcessorConfig(IsClientNonAuthoritativeMode(NonAuthoritativeMode.Static)) },
                { typeof (LocalTransformUpdaterBehaviour), new VisualizerPreProcessorConfig(ShouldClientAddAuthoritativeBehaviours) },
                { typeof (LocalTransformTeleportBehaviour), new VisualizerPreProcessorConfig(ShouldClientAddAuthoritativeBehaviours) }
            };
        }

        protected override Dictionary<Type, VisualizerPreProcessorConfig> GetFSimVisualizersToAdd()
        {
            return new Dictionary<Type, VisualizerPreProcessorConfig>
            {
                { typeof (ExactGlobalTransformBehaviour), new VisualizerPreProcessorConfig(IsFSimNonAuthoritativeMode(NonAuthoritativeMode.Exact, true)) },
                { typeof (ExactLocalTransformBehaviour), new VisualizerPreProcessorConfig(IsFSimNonAuthoritativeMode(NonAuthoritativeMode.Exact)) },
                { typeof (LerpGlobalTransformBehaviour), new VisualizerPreProcessorConfig(IsFSimNonAuthoritativeMode(NonAuthoritativeMode.Lerp, true)) },
                { typeof (LerpLocalTransformBehaviour), new VisualizerPreProcessorConfig(IsFSimNonAuthoritativeMode(NonAuthoritativeMode.Lerp)) },
                { typeof (StaticGlobalTransformBehaviour), new VisualizerPreProcessorConfig(IsFSimNonAuthoritativeMode(NonAuthoritativeMode.Static, true)) },
                { typeof (StaticLocalTransformBehaviour), new VisualizerPreProcessorConfig(IsFSimNonAuthoritativeMode(NonAuthoritativeMode.Static)) },
                { typeof (LocalTransformUpdaterBehaviour), new VisualizerPreProcessorConfig(ShouldFSimAddAuthoritativeBehaviours) },
                { typeof (LocalTransformTeleportBehaviour), new VisualizerPreProcessorConfig(ShouldFSimAddAuthoritativeBehaviours) }
            };
        }

        protected bool ShouldAddParentBehaviours()
        {
            return GameObjectCanBeParent;
        }

        protected bool ShouldAddParentedBehaviours()
        {
            return GameObjectCanBeParented;
        }

        protected bool ShouldAddNonParentedBehaviours()
        {
            return !GameObjectCanBeParented;
        }

        protected Func<bool> IsClientNonAuthoritativeMode(NonAuthoritativeMode mode, bool checkForGlobalFallback = false)
        {
            return () => (!checkForGlobalFallback || ClientShouldAddGlobalFallback()) && ClientNonAuthoritativeMode == mode;
        }

        protected Func<bool> IsFSimNonAuthoritativeMode(NonAuthoritativeMode mode, bool checkForGlobalFallback = false)
        {
            return () => (!checkForGlobalFallback || FSimShouldAddGlobalFallback()) &&  FSimNonAuthoritativeMode == mode;
        }

        protected bool ClientShouldAddGlobalFallback()
        {
            return ClientEnableGlobalFallback && GameObjectCanBeParented;
        }

        protected bool FSimShouldAddGlobalFallback()
        {
            return FSimEnableGlobalFallback && GameObjectCanBeParented;
        }

        protected bool ShouldClientAddAuthoritativeBehaviours()
        {
            if (!ClientCanBeAuthoritative)
            {
                return false;
            }
            if (ClientNonAuthoritativeMode == NonAuthoritativeMode.Static)
            {
                Logger.WarnFormat("A static gameObject should not be authoritative. (GameObject {0}).", gameObject.name);
                return false;
            }
            return true;
        }

        protected bool ShouldFSimAddAuthoritativeBehaviours()
        {
            if (!FSimCanBeAuthoritative)
            {
                return false;
            }
            if (FSimNonAuthoritativeMode == NonAuthoritativeMode.Static)
            {
                Logger.WarnFormat("A static gameObject should not be authoritative. (GameObject {0}).", gameObject.name);
                return false;
            }
            return true;
        }

        protected void AddTransformOffsetRegistry(GameObject targetGameObject, Type visualizerType)
        {
            TransformOffsetsRegistry.AddOffsetsRegistry(targetGameObject);
        }

        public float GetInterpolationDelayInSeconds()
        {
            return InterpolationDelayInSeconds;
        }

        public float GetMinAngleToInterpolateBetween()
        {
            return MinAngleToInterpolateBetween;
        }

        public float GetMinDistanceToInterpolateBetween()
        {
            return MinDistanceToInterpolateBetween;
        }

        public float GetMaxSecondsToInterpolateAfterLastUpdate()
        {
            return MaxSecondsToInterpolateAfterLastUpdate;
        }

        public bool GetUseCenterOfMassPivot()
        {
            return UseCenterOfMassPivot;
        }

        public Vector3 GetPivotOffset()
        {
            return PivotOffset;
        }

        public float GetNetworkUpdatePeriodThreshold()
        {
            return NetworkUpdatePeriodThreshold;
        }

        public float GetPositionNetworkUpdateSquareDistanceThreshold()
        {
            return PositionNetworkUpdateSquareDistanceThreshold;
        }

        public float GetRotationNetworkUpdateAngleThreshold()
        {
            return RotationNetworkUpdateAngleThreshold;
        }
    }
}