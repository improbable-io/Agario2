using UnityEngine;
using System.Collections.Generic;
using Improbable.Corelibrary.Transforms.Offsets;
using Improbable.Unity.Visualizer;
using Transform = Improbable.Corelibrary.Transforms.Transform;

namespace Improbable.CoreLibrary.Transforms.Offsets
{
    /// <summary>
    /// The TransformOffsetsOneTimeUpdater is used to update the TransformKeyOffsetsState. The behaviour will only update 
    /// the state for each offset at most once.
    /// </summary>
    public class TransformOffsetsOneTimeUpdater : MonoBehaviour
    {
        [Require] protected TransformKeyOffsetsStateWriter TransformKeyOffsetsStateWriter;

        protected readonly IDictionary<string, Transform> TransformDictionary = new Dictionary<string, Transform>();

        protected TransformOffsetsRegistry Registry;
        protected UnityEngine.Transform CachedTransform;

        protected virtual void Awake()
        {
            CachedTransform = transform;
            Registry = GetComponent<TransformOffsetsRegistry>();
            if (Registry == null)
            {
                throw new MissingComponentException(string.Format("{0} on {1} expects a {2}.", GetType().Name, gameObject.name, typeof(TransformOffsetsRegistry).Name));
            }
            Registry.OnOffsetAdded += OnOffsetAdded;
            Registry.OnOffsetRemoved += OnOffsetRemoved;
        }

        protected virtual void OnEnable()
        {
            UpdateState();
        }

        protected virtual void OnOffsetAdded(string key, UnityEngine.Transform offsetTransform)
        {
            TransformDictionary.Add(key, TransformOffsetsUtil.CalculateRelativeTransform(offsetTransform, transform));
            UpdateState();
        }

        protected virtual void OnOffsetRemoved(string key)
        {
            TransformDictionary.Remove(key);
            UpdateState();
        }

        protected void UpdateState()
        {
            if (TransformKeyOffsetsStateWriter != null)
            {
                TransformKeyOffsetsStateWriter.Update
                    .KeyMap(TransformDictionary)
                    .FinishAndSend();
            }
        }
    }
}