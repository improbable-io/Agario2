using Improbable.Math;
using UnityEngine;

namespace Improbable.Unity.Common.Core.Math
{
    public static class Vector3UnityExtension
    {
        public static UnityEngine.Vector3 ToUnityVector(this Vector3d nativeVector3D)
        {
            return new UnityEngine.Vector3((float) nativeVector3D.X, (float) nativeVector3D.Y, (float) nativeVector3D.Z);
        }

        public static Vector3d ToNativeVector(this UnityEngine.Vector3 unityVector3)
        {
            return new Vector3d(unityVector3.x, unityVector3.y, unityVector3.z);
        }

        public static UnityEngine.Vector3 ToUnityVector(this Vector3f nativeVector3)
        {
            return new UnityEngine.Vector3(nativeVector3.X, nativeVector3.Y, nativeVector3.Z);
        }

        public static Vector3f ToNativeVector3f(this UnityEngine.Vector3 unityVector3)
        {
            return new Vector3f(unityVector3.x, unityVector3.y, unityVector3.z);
        }

        public static Quaternion ToUnityQuaternion(this Vector3d nativeVector3D)
        {
            return Quaternion.Euler(nativeVector3D.ToUnityVector());
        }

        public static Vector3f ToVector3f(this Vector3d nativeVector3d)
        {
            return new Vector3f((float) nativeVector3d.X, (float) nativeVector3d.Y, (float) nativeVector3d.Z);
        }

        public static Vector3d ToVector3d(this Vector3f nativeVector3f)
        {
            return new Vector3d(nativeVector3f.X, nativeVector3f.Y, nativeVector3f.Z);
        }

        public static bool IsFinite(this UnityEngine.Vector3 unityVector3)
        {
            return !float.IsInfinity(unityVector3.x) && !float.IsInfinity(unityVector3.y) && !float.IsInfinity(unityVector3.z) &&
                   !float.IsNaN(unityVector3.x) && !float.IsNaN(unityVector3.y) && !float.IsNaN(unityVector3.z);
        }
    }
}