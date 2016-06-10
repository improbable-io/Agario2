using UnityEngine;

namespace Improbable.CoreLibrary.Transforms.Offsets
{
    /// <summary>
    /// The Transform Offsets Notifier is added by the TransformOffsetsRegistry at pre-processing time on all children
    /// gameObjects that conform to a specific name format.
    /// 
    /// The RegistryDepth specifies how far up the transform parent chain the main game object is and is set when the
    /// TransformOffsetsRegistry adds this behaviour.
    /// 
    /// <seealso cref="TransformOffsetsRegistry"/>
    /// </summary>
    public class TransformOffsetsNotifier : MonoBehaviour
    {
        public int RegistryDepth;

        protected virtual void Awake()
        {
            var registry = TryGetRegistry();
            if (registry != null)
            {
                registry.NotifyTaggedOffset(gameObject);
            }
            else
            {
                throw new MissingComponentException(string.Format("{0} expects a {1} at depth {2}.", GetType().Name, typeof(TransformOffsetsRegistry).Name, RegistryDepth));
            }
        }

        protected virtual void OnDestroy()
        {
            var registry = TryGetRegistry();
            if (registry != null)
            {
                registry.NotifyTaggedOffsetDestroy(gameObject);
            }
            // Not fatal if the registry is not there -- the object is probably being destroyed.
        }

        protected TransformOffsetsRegistry TryGetRegistry()
        {
            var currentTransform = transform;
            for (int currentDepth = 0; currentDepth < RegistryDepth; currentDepth ++)
            {
                currentTransform = currentTransform.parent;
            }
            return currentTransform.GetComponent<TransformOffsetsRegistry>();
        }
    }
}
