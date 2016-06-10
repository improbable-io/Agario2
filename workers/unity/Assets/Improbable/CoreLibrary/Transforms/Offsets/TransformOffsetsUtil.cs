using Improbable.Corelib.Util;
using Improbable.Unity.Common.Core.Math;
using UnityEngine;

namespace Improbable.CoreLibrary.Transforms.Offsets
{
    /// <summary>
    /// This class provides several utility functions for Tranform Offsets.
    /// </summary>
    public static class TransformOffsetsUtil
    {
        public const string TRANSFORM_OFFSET_OBJECT_NAME_PREFIX = "#";

        public static Corelibrary.Transforms.Transform CalculateRelativeTransform(Transform childTransform,
            Transform parentTransform)
        {
            var inverseParentRotation = Quaternion.Inverse(parentTransform.rotation);
            var rotation = inverseParentRotation * childTransform.rotation;
            var position = inverseParentRotation * (childTransform.position - parentTransform.position);

            return new Corelibrary.Transforms.Transform(position.ToNativeVector(), rotation.ToNativeQuaternion());
        }
    }
}