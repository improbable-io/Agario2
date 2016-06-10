using UnityEngine;
using Improbable.Corelib.Interpolation;
using Improbable.Corelib.Util;
using Improbable.Math;

namespace Improbable.CoreLibrary.Transforms
{
    /// <summary>
    /// The TransformLinearInterpolator uses pivot-based transform interpolation to linearly interpolate between transform
    /// positions.
    /// 
    /// The pivot-based interpolator, assumes that the pivot moves linearly (instead of the position), and uses that to
    /// determine the actual interpolated position.
    /// </summary>
    public class TransformLinearInterpolator : DelayedLinearInterpolator<TransformData>
    {
        public TransformLinearInterpolator(float interpolationDelaySeconds): base(interpolationDelaySeconds) { }

        protected override TransformData Interpolate(TransformData currentValue, TransformData nextValue, float progressRatio)
        {
            var rotation = RotationLerp(currentValue.Rotation, nextValue.Rotation, progressRatio);
            var pivot = PositionLerp(currentValue.Pivot, nextValue.Pivot, progressRatio);
            var pivotGlobal = PositionLerp(currentValue.GlobalPivot(), nextValue.GlobalPivot(), progressRatio);
            var position = pivotGlobal - rotation * pivot;

            return new TransformData(position, rotation, pivot);
        }

        private static Corelib.Math.Quaternion RotationLerp(Corelib.Math.Quaternion currentValue, Corelib.Math.Quaternion nextValue, float progressRatio)
        {
            return Quaternion.Lerp(currentValue.ToUnityQuaternion(), nextValue.ToUnityQuaternion(), progressRatio).ToNativeQuaternion();
        }

        private static Vector3d PositionLerp(Vector3d currentValue, Vector3d nextValue, float progressRatio)
        {
            var distanceDelta = nextValue - currentValue;
            return distanceDelta * progressRatio + currentValue;
        }
    }

    public struct TransformData
    {
        public readonly Vector3d Position;
        public readonly Corelib.Math.Quaternion Rotation;
        public readonly Vector3d Pivot;

        public TransformData(Vector3d position, Corelib.Math.Quaternion rotation, Vector3d pivot)
        {
            Position = position;
            Rotation = rotation;
            Pivot = pivot;
        }

        public Vector3d GlobalPivot()
        {
            return Rotation * Pivot + Position;
        }
    }
}
