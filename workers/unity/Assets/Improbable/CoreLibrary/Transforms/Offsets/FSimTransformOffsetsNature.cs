using System;
using UnityEngine;
using System.Collections.Generic;
using Improbable.Corelib.Physical;
using Improbable.Corelib.PrefabExporting.PreProcessors;
using Improbable.Unity.Export;

namespace Improbable.CoreLibrary.Transforms.Offsets
{
    /// <summary>
    /// The FSimTransformOffsetsNature adds the behaviours necessary for the FSimTransformOffsetsNature.
    /// </summary>
    [KeepOnExportedPrefab]
    public class FSimTransformOffsetsNature : PreProcessorBase, ITransformOffsetsUpdateSettings
    {
        [Tooltip("Static should be used for objects whose offset(s) do not change. Dynamic should be used for objects whose offset(s) may change.")]
        public UpdateMode OffsetUpdateMode;

        [Header("Dynamic Update Configuration")]
        [Tooltip("The minimum angle in degrees from the last sent rotation before sending a new update.")]
        public float RotationNetworkUpdateAngleThreshold = DefaultPhysicalParameters.RotationNetworkUpdateAngleThreshold;

        [Tooltip("The minimum square distance from the last sent position before sending a new update.")] 
        public float PositionNetworkUpdateSquareDistanceThreshold = DefaultPhysicalParameters.RotationNetworkUpdateSquareDistanceThreshold;

        [Tooltip("The minimum number of seconds between sending updates.")] 
        public float NetworkUpdatePeriodThresholdInSeconds = DefaultPhysicalParameters.SlottedNetworkUpdatePeriodThreshold;

        [Tooltip("Enable this to only update when the transform offset is used. (Recommended)")] 
        public bool OnlyUpdateWhenUsed = true;

        public enum UpdateMode
        {
            Static, Dynamic
        }

        protected override Dictionary<Type, VisualizerPreProcessorConfig> GetFSimVisualizersToAdd()
        {
            return new Dictionary<Type, VisualizerPreProcessorConfig>
            {
                { typeof (TransformOffsetsRegistry), new VisualizerPreProcessorConfig(addVisualizer: AddTransformOffsetRegistry) },
                { typeof (TransformOffsetsOneTimeUpdater), new VisualizerPreProcessorConfig(IsUpdateMode(UpdateMode.Static)) },
                { typeof (TransformOffsetsUpdater), new VisualizerPreProcessorConfig(IsUpdateMode(UpdateMode.Dynamic)) }
            };
        }

        protected void AddTransformOffsetRegistry(GameObject targetGameObject, Type visualizerType)
        {
            TransformOffsetsRegistry.AddOffsetsRegistry(targetGameObject);
        }

        protected Func<bool> IsUpdateMode(UpdateMode mode)
        {
            return () => OffsetUpdateMode == mode;
        }

        public float GetRotationNetworkUpdateAngleThreshold()
        {
            return RotationNetworkUpdateAngleThreshold;
        }

        public float GetPositionNetworkUpdateSquareDistanceThreshold()
        {
            return PositionNetworkUpdateSquareDistanceThreshold;
        }

        public float GetNetworkUpdatePeriodThresholdInSeconds()
        {
            return NetworkUpdatePeriodThresholdInSeconds;
        }

        public bool GetOnlyUpdateWhenUsed()
        {
            return OnlyUpdateWhenUsed;
        }
    }
}