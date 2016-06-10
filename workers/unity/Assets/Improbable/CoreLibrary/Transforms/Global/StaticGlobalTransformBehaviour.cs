using Improbable.Corelib.Util;
using Improbable.Corelibrary.Transforms.Global;
using Improbable.CoreLibrary.CoordinateRemapping;
using UnityEngine;
using Improbable.Unity.Visualizer;

namespace Improbable.CoreLibrary.Transforms.Global
{
    /// <summary>
    /// The StaticGlobalTransformBehaviour uses the GlobalTransformState to set a static object's position and rotation.
    /// </summary>
    [DontAutoEnable]
    public class StaticGlobalTransformBehaviour : MonoBehaviour, IGlobalModeBehaviour
    {
        [Require] protected GlobalTransformStateReader GlobalTransformStateReader;

        protected virtual void OnEnable()
        {
            transform.position = GlobalTransformStateReader.Position.RemapGlobalToUnityVector();
            transform.rotation = GlobalTransformStateReader.Rotation.ToUnityQuaternion();
        }
    }
}
