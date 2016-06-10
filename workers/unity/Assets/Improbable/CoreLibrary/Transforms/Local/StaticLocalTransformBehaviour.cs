using Improbable.Corelib.Util;
using Improbable.Corelibrary.Transforms;
using Improbable.CoreLibrary.CoordinateRemapping;
using Improbable.Unity;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Improbable.CoreLibrary.Transforms.Local
{
    /// <summary>
    /// The StaticLocalTransformBehaviour uses the TransformState to set a static's object position and rotation.
    /// </summary>
    [DontAutoEnable]
    public class StaticLocalTransformBehaviour : MonoBehaviour, ILocalModeBehaviour
    {
        [Require] protected TransformStateReader TransformStateReader;

        protected virtual void OnEnable()
        {
            if (HasParent())
            {
                transform.localPosition = TransformStateReader.LocalPosition.ToUnityVector();
                transform.localRotation = TransformStateReader.LocalRotation.ToUnityQuaternion();
            }
            else
            {
                transform.position = TransformStateReader.LocalPosition.RemapGlobalToUnityVector();
                transform.rotation = TransformStateReader.LocalRotation.ToUnityQuaternion();
            }
        }

        protected bool HasParent()
        {
            return TransformStateReader.Parent.HasValue;
        }
    }
}