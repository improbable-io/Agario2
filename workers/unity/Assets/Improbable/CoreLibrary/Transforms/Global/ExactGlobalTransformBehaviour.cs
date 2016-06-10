using UnityEngine;
using Improbable.Corelib.Util;
using Improbable.Corelibrary.Transforms.Global;
using Improbable.CoreLibrary.CoordinateRemapping;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Quaternion = Improbable.Corelib.Math.Quaternion;

namespace Improbable.CoreLibrary.Transforms.Global
{
    /// <summary>
    /// The ExactGlobalTransformBehaviour sets the unity position to correspond to exactly the value
    /// that the state is. It uses the GlobalTransformState.
    /// 
    /// <seealso cref="IGlobalModeBehaviour"/>
    /// </summary>
    [DontAutoEnable]
    public class ExactGlobalTransformBehaviour : MonoBehaviour, IGlobalModeBehaviour
    {
        [Require] protected GlobalTransformStateReader GlobalTransformStateReader;

        protected virtual void OnEnable()
        {
            GlobalTransformStateReader.PositionUpdated += UpdatePosition;
            GlobalTransformStateReader.RotationUpdated += UpdateRotation;
        }

        protected virtual void OnDisable()
        {
            GlobalTransformStateReader.PositionUpdated -= UpdatePosition;
            GlobalTransformStateReader.RotationUpdated -= UpdateRotation;
        }

        protected virtual void UpdatePosition(Coordinates coordinates)
        {
            transform.position = coordinates.RemapGlobalToUnityVector();
        }

        protected virtual void UpdateRotation(Quaternion rotation)
        {
            transform.rotation = rotation.ToUnityQuaternion();
        }
    }
}