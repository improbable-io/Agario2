using System;
using Improbable.Corelib.Util;
using Improbable.Corelibrary.Transforms.Global;
using Improbable.CoreLibrary.CoordinateRemapping;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Quaternion = Improbable.Corelib.Math.Quaternion;

namespace Improbable.CoreLibrary.Transforms.Global
{
    /// <summary>
    /// The LerpGlobalTransformBehaviour is an instantiation of the AbstractLerpTransformBehaviour which uses
    /// the GlobalTransformState.
    /// 
    /// <seealso cref="IGlobalModeBehaviour"/>
    /// <seealso cref="AbstractLerpTransformBehaviour"/>
    /// </summary>
    [DontAutoEnable]
    public class LerpGlobalTransformBehaviour : AbstractLerpTransformBehaviour, IGlobalModeBehaviour
    {
        [Require] protected GlobalTransformStateReader GlobalTransformStateReader;

        protected virtual void Update()
        {
            DoUpdate();
        }
        
        protected override TransformData GetCurrentStateData()
        {
            var positionCoordinates = GlobalTransformStateReader.Position;
            var positionVector3D = new Vector3d(positionCoordinates.X, positionCoordinates.Y, positionCoordinates.Z);
            return new TransformData(positionVector3D, GlobalTransformStateReader.Rotation,
                GlobalTransformStateReader.Pivot);
        }

        protected override float GetCurrentStateTimestamp()
        {
            return GlobalTransformStateReader.Timestamp;
        }

        protected override bool IsAuthoritative()
        {
            // Always in non-authoritative mode
            return false;
        }

        protected override void RegisterPropertyListener(Action action)
        {
            GlobalTransformStateReader.PropertyUpdated += action;
        }

        protected override void UnregisterPropertyListener(Action action)
        {
            GlobalTransformStateReader.PropertyUpdated -= action;
        }

        protected override void RegisterAuthorityListener(Action<bool> action)
        {
            action(false);
        }

        protected override void UnregisterAuthorityListener(Action<bool> action) { }

        protected override Vector3d CurrentPosition()
        {
            return CachedTransform.position.RemapUnityVectorToGlobalVector();
        }

        protected override Quaternion CurrentRotation()
        {
            return CachedTransform.rotation.ToNativeQuaternion();
        }

        protected override void SetPosition(Vector3d newPosition)
        {
            CachedTransform.position = newPosition.RemapGlobalToUnityVector();
        }

        protected override void SetRotation(Quaternion newRotation)
        {
            CachedTransform.rotation = newRotation.ToUnityQuaternion();
        }
    }
}