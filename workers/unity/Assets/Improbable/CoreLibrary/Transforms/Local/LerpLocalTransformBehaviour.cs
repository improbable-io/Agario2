using System;
using Improbable.Corelib.Util;
using Improbable.Corelibrary.Transforms;
using Improbable.CoreLibrary.CoordinateRemapping;
using Improbable.Math;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using Quaternion = Improbable.Corelib.Math.Quaternion;

namespace Improbable.CoreLibrary.Transforms.Local
{
    /// <summary>
    /// The LerpLocalTransformBehaviour is an instantiation of the AbstractLerpTransformBehaviour which uses
    /// the TransformState.
    /// 
    /// <seealso cref="ILocalModeBehaviour"/>
    /// <seealso cref="AbstractLerpTransformBehaviour"/>
    /// </summary>
    [DontAutoEnable]
    public class LerpLocalTransformBehaviour : AbstractLerpTransformBehaviour, ILocalModeBehaviour
    {
        [Require] protected TransformStateReader TransformStateReader;

        protected virtual void Update()
        {
            DoUpdate();
        }

        protected override void RegisterOtherListeners()
        {
            TransformStateReader.ParentUpdated += OnParentUpdated;
        }

        protected override void UnregisterOtherListeners()
        {
            TransformStateReader.ParentUpdated -= OnParentUpdated;
        }

        protected void OnParentUpdated(Parent? parent)
        {
            Reset();
        }

        protected bool IsParented()
        {
            return TransformStateReader.Parent.HasValue;
        }

        protected override TransformData GetCurrentStateData()
        {
            return new TransformData(TransformStateReader.LocalPosition, TransformStateReader.LocalRotation, TransformStateReader.Pivot);
        }

        protected override float GetCurrentStateTimestamp()
        {
            return TransformStateReader.Timestamp;
        }

        protected override bool IsAuthoritative()
        {
            return TransformStateReader.IsAuthoritativeHere;
        }

        protected override void RegisterPropertyListener(Action action)
        {
            TransformStateReader.PropertyUpdated += action;
        }

        protected override void UnregisterPropertyListener(Action action)
        {
            TransformStateReader.PropertyUpdated -= action;
        }

        protected override void RegisterAuthorityListener(Action<bool> action)
        {
            TransformStateReader.AuthorityChanged += action;
        }

        protected override void UnregisterAuthorityListener(Action<bool> action)
        {
            TransformStateReader.AuthorityChanged -= action;
        }

        protected override Vector3d CurrentPosition()
        {
            if (!IsParented())
            {
                return CachedTransform.position.RemapUnityVectorToGlobalVector();
            }
            else
            {
                return CachedTransform.localPosition.ToNativeVector();
            }
            
        }

        protected override Quaternion CurrentRotation()
        {
            if (!IsParented())
            {
                return CachedTransform.rotation.ToNativeQuaternion();
            }
            else
            {
                return CachedTransform.localRotation.ToNativeQuaternion();
            }
        }

        protected override void SetPosition(Vector3d newPosition)
        {
            if (!IsParented())
            {
                CachedTransform.position = newPosition.RemapGlobalToUnityVector();
            }
            else
            {
                CachedTransform.localPosition = newPosition.ToUnityVector();
            }
        }

        protected override void SetRotation(Quaternion newRotation)
        {
            if (!IsParented())
            {
                CachedTransform.rotation = newRotation.ToUnityQuaternion();
            }
            else
            {
                CachedTransform.localRotation = newRotation.ToUnityQuaternion();
            }
        }
    }
}