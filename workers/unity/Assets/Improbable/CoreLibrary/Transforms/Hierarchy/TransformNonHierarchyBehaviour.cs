using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Improbable.Corelibrary.Transforms;
using Improbable.CoreLibrary.Transforms.Local;
using Improbable.Unity.Visualizer;

namespace Improbable.CoreLibrary.Transforms.Hierarchy
{
    /// <summary>
    /// The TransformNonHierarchyBehaviour is an optimized TransformChildHierarchyBehaviour which does not
    /// alternate between Local and Global modes and defaulst to the LocalMode always.
    /// <seealso cref="TransformChildHierarchyBehaviour"/>
    /// </summary>
    public class TransformNonHierarchyBehaviour : MonoBehaviour
    {
        [Require] protected TransformStateReader TransformStateReaderCheck;

        protected readonly List<object> LocalModeVisualizers = new List<object>();

        protected virtual void Awake()
        {
            var localVisualizers = GetComponents<ILocalModeBehaviour>();
            for (var index = 0; index < localVisualizers.Length; index++)
            {
                LocalModeVisualizers.Add(localVisualizers[index]);
            }
        }

        protected virtual void OnEnable()
        {
            gameObject.GetEntityObject().TryEnableVisualizers(LocalModeVisualizers);
        }

        protected virtual void OnDisable()
        {
            gameObject.GetEntityObject().DisableVisualizers(LocalModeVisualizers);
        }
    }
}