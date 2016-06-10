using Improbable.Corelib.Util;
using UnityEngine;
using Improbable.Corelibrary.Transforms;
using Improbable.CoreLibrary.CoordinateRemapping;
using Improbable.Math;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using Quaternion = Improbable.Corelib.Math.Quaternion;
using Transform = UnityEngine.Transform;

namespace Improbable.CoreLibrary.Transforms.Local
{
    /// <summary>
    /// The ExactLocalTransformBehaviour sets the unity position to correspond to exactly the value
    /// that the state is. It uses the TransformState.
    ///
    /// <seealso cref="ILocalModeBehaviour"/>
    /// </summary>
    [DontAutoEnable]
    public class ExactLocalTransformBehaviour : MonoBehaviour, ILocalModeBehaviour
    {
        [Require] protected TransformStateReader TransformStateReader;

        protected Transform CachedTransform { get; set; }

        protected virtual void Awake()
        {
            CachedTransform = transform;
        }

        protected virtual void OnEnable()
        {
            TransformStateReader.AuthorityChanged += OnAuthorityChanged;
        }

        protected virtual void OnDisable()
        {
            UnregisterListeners();
        }

        protected void OnAuthorityChanged(bool isAuthoritativeHere)
        {
            if (!isAuthoritativeHere)
            {
                RegisterListeners();
            }
            else
            {
                UnregisterListeners();
            }
        }

        protected void RegisterListeners()
        {
            TransformStateReader.LocalPositionUpdated += UpdateLocalPosition;
            TransformStateReader.LocalRotationUpdated += UpdateLocalRotation;
        }

        protected void UnregisterListeners()
        {
            TransformStateReader.LocalPositionUpdated -= UpdateLocalPosition;
            TransformStateReader.LocalRotationUpdated -= UpdateLocalRotation;
        }

        protected virtual void UpdateLocalPosition(Vector3d localPosition)
        {
            if (HasParent())
            {
                CachedTransform.localPosition = localPosition.ToUnityVector();
            }
            else
            {
                CachedTransform.position = localPosition.RemapGlobalToUnityVector();
            }
        }

        protected virtual void UpdateLocalRotation(Quaternion localRotation)
        {
            if (HasParent())
            {
                CachedTransform.localRotation = localRotation.ToUnityQuaternion();
            }
            else
            {
                CachedTransform.rotation = localRotation.ToUnityQuaternion();
            }
        }

        protected bool HasParent()
        {
            return TransformStateReader.Parent.HasValue;
        }
    }
}