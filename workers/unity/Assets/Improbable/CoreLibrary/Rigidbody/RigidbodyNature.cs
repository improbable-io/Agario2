using System;
using UnityEngine;
using System.Collections.Generic;
using Improbable.Core.GameLogic.Visualizers;
using Improbable.Corelib.Physical;
using Improbable.Corelib.PrefabExporting.PreProcessors;
using Improbable.CoreLibrary.Transforms;
using Improbable.Unity.Export;

namespace Improbable.CoreLibrary.Rigidbody
{
    /// <summary>
    /// The RigidbodyNature adds the Unity Behaviours for the RigidbodyNature.
    /// 
    /// It requires the Transform Nature.
    /// 
    /// <seealso cref="TransformNature"/>
    /// </summary>
    [KeepOnExportedPrefab]
    public class RigidbodyNature : PreProcessorBase
    {
        [Header("Client Settings")]
        [Tooltip("Whether to add the rigidbody on clients.")]
        public bool AddRigidbodyOnClients = false;

        [Header("FSim Settings")]
        [Tooltip("Whether to add the rigidbody on FSims.")]
        public bool AddRigidbodyOnFSims = true;

        [Header("General Settings")]
        [Tooltip("Whether the rigidbody added should be removed when this object's transform is parented.")]
        public bool DisableRigidbodyWhenParented = true;
        [Tooltip("Whether the rigidbody should be set to kinematic on non-authoritative engines.")]
        public bool NonAuthoritativeRigidbodyIsKinematic = false;

        private TransformNature cachedTransformNature;
        protected TransformNature CachedTransformNature
        {
            get
            {
                if (cachedTransformNature == null)
                {
                    cachedTransformNature = GetComponent<TransformNature>();
                    if (cachedTransformNature == null)
                    {
                        throw new MissingComponentException(string.Format("{0} on {1} expects a {2}.", GetType().Name, gameObject.name, typeof(TransformNature).Name));
                    }
                }
                return cachedTransformNature;
            }
        }

        protected override Dictionary<Type, VisualizerPreProcessorConfig> GetClientVisualizersToAdd()
        {
            return new Dictionary<Type, VisualizerPreProcessorConfig>
            {
                { typeof (RigidbodyVisualizer), new VisualizerPreProcessorConfig(ShouldAddRigidbodyOnClients, AddRigidbodyVisualizer) },
                { typeof (RigidbodySync), new VisualizerPreProcessorConfig(ShouldAddAuthoritativeBehavioursOnClients) },
                { typeof (DisableRigidbodyWhenParentedBehaviour), new VisualizerPreProcessorConfig(ShouldAddDisableRigidbodyVisualizerOnClients) }
            };
        }

        protected override Dictionary<Type, VisualizerPreProcessorConfig> GetFSimVisualizersToAdd()
        {
            return new Dictionary<Type, VisualizerPreProcessorConfig>
            {
                { typeof (RigidbodyVisualizer), new VisualizerPreProcessorConfig(ShouldAddRigidbodyOnFSims, AddRigidbodyVisualizer) },
                { typeof (RigidbodySync), new VisualizerPreProcessorConfig(ShouldAddAuthoritativeBehavioursOnFSims) },
                { typeof (DisableRigidbodyWhenParentedBehaviour), new VisualizerPreProcessorConfig(ShouldAddDisableRigidbodyVisualizerOnFSims) }
            };
        }

        private void AddRigidbodyVisualizer(GameObject targetGameObject, Type behaviourType)
        {
            targetGameObject.AddComponent<RigidbodyVisualizer>().NonAuthoritativeRigidbodyIsKinematic = NonAuthoritativeRigidbodyIsKinematic;
        }

        protected bool ShouldAddRigidbodyOnClients()
        {
            return AddRigidbodyOnClients;
        }

        protected bool ShouldAddRigidbodyOnFSims()
        {
            return AddRigidbodyOnFSims;;
        }

        protected bool ShouldAddAuthoritativeBehavioursOnClients()
        {
            return ShouldAddRigidbodyOnClients() && CachedTransformNature.ClientCanBeAuthoritative;
        }

        protected bool ShouldAddAuthoritativeBehavioursOnFSims()
        {
            return ShouldAddRigidbodyOnFSims() && CachedTransformNature.FSimCanBeAuthoritative;
        }

        protected bool ShouldAddDisableRigidbodyVisualizerOnClients()
        {
            return ShouldAddRigidbodyOnClients() && DisableRigidbodyWhenParented && CachedTransformNature.GameObjectCanBeParented;
        }

        protected bool ShouldAddDisableRigidbodyVisualizerOnFSims()
        {
            return ShouldAddRigidbodyOnFSims() && DisableRigidbodyWhenParented && CachedTransformNature.GameObjectCanBeParented;
        }
    }
}
