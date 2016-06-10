using System;
using UnityEngine;
using System.Collections.Generic;
using log4net;

namespace Improbable.CoreLibrary.Transforms.Offsets
{
    /// <summary>
    /// The TransformOffsetsRegistry is holds the information about all the TaggedOffsets present in this
    /// gameObject. It allows other behaviours to add callbacks which listen to Add and Remove callbacks.
    /// 
    /// At pre-processor time, the registry adds TransformOffsetsNotifier to all the child objects which start
    /// with a particular prefix.
    /// 
    /// <seealso cref="TransformOffsetsNotifier"/>
    /// </summary>
    public class TransformOffsetsRegistry : MonoBehaviour
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TransformOffsetsRegistry));

        public readonly IDictionary<string, Transform> TaggedOffsets = new Dictionary<string, Transform>();

        protected readonly List<Action<string, Transform>> onAddCallbacks = new List<Action<string, Transform>>();
        protected readonly List<Action<string>> onRemoveCallbacks = new List<Action<string>>();

        public event Action<string, Transform> OnOffsetAdded
        {
            add
            {
                onAddCallbacks.Add(value);
                foreach (var keyValue in TaggedOffsets)
                {
                    value(keyValue.Key, keyValue.Value);
                }
            }
            remove { onAddCallbacks.Remove(value); }
        }

        public event Action<string> OnOffsetRemoved
        {
            add
            {
                onRemoveCallbacks.Add(value);
            }
            remove { onRemoveCallbacks.Remove(value); }
        }

        public static void AddOffsetsRegistry(GameObject targetGameObject)
        {
            if (targetGameObject.GetComponent<TransformOffsetsRegistry>() == null)
            {
                RecursiveTagOffsetChildren(targetGameObject.transform);
                targetGameObject.AddComponent<TransformOffsetsRegistry>();
            }
        }

        protected static void RecursiveTagOffsetChildren(Transform transform, int depth = 1)
        {
            // foreach does extra allocations in Mono
            for (var index = 0; index < transform.childCount; index++)
            {
                var childTransform = transform.GetChild(index);
                var childObject = childTransform.gameObject;
                if (IsTaggedOffset(childObject))
                {
                    TagOffsetObject(childObject, depth);
                }
                RecursiveTagOffsetChildren(childTransform, depth + 1);
            }
        }

        protected static void TagOffsetObject(GameObject gameObject, int depth)
        {
            var notifier = gameObject.AddComponent<TransformOffsetsNotifier>();
            notifier.RegistryDepth = depth;
        }

        protected static bool IsTaggedOffset(GameObject gameObject)
        {
            return gameObject.name.StartsWith(TransformOffsetsUtil.TRANSFORM_OFFSET_OBJECT_NAME_PREFIX);
        }

        protected static string TagOffsetName(GameObject gameObject)
        {
            return IsTaggedOffset(gameObject) ? gameObject.name.Remove(0, TransformOffsetsUtil.TRANSFORM_OFFSET_OBJECT_NAME_PREFIX.Length) : gameObject.name;
        }

        protected void TriggerOnAddCallbacks(string key, Transform targetTransform)
        {
            for (var index = 0; index < onAddCallbacks.Count; index++)
            {
                onAddCallbacks[index](key, targetTransform);
            }
        }

        protected void TriggerOnRemoveCallbacks(string key)
        {
            for (var index = 0; index < onRemoveCallbacks.Count; index++)
            {
                onRemoveCallbacks[index](key);
            }
        }

        public virtual void NotifyTaggedOffset(GameObject offsetObject)
        {
            var offsetName = TagOffsetName(offsetObject);
            if (TaggedOffsets.ContainsKey(offsetName))
            {
                Logger.WarnFormat("Multiple tagged transform offest objects in gameObject {0} with offset key {1}.", gameObject.name, offsetName);
                return;
            }
            var offsetTransform = offsetObject.transform;
            TaggedOffsets.Add(offsetName, offsetTransform);
            TriggerOnAddCallbacks(offsetName, offsetTransform);
        }

        public virtual void NotifyTaggedOffsetDestroy(GameObject offsetObject)
        {
            var offsetName = TagOffsetName(offsetObject);
            TaggedOffsets.Remove(offsetName);
            TriggerOnRemoveCallbacks(offsetName);
        }
    }
}