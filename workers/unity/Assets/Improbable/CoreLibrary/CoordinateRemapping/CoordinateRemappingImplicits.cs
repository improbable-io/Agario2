using Improbable.CoreLibrary.CoordinateRemapping.DetermineOrigin;
using UnityEngine;
using Improbable.Math;

namespace Improbable.CoreLibrary.CoordinateRemapping
{
    /// <summary>
    /// This class contains the Implicit conversions for Coordinate Remapping using the CoordinateRemappingBehaviour
    /// class.
    /// 
    /// <seealso cref="CoordinateRemappingBehaviour"/>
    /// </summary>
    public static class CoordinateRemappingImplicits
    {
        // Types are fully qualified here for clarity.

        /// <summary>
        /// Converts a Vector3d which represents a global position to a Unity local position via
        /// coordinate remapping using the IDetermineOriginStrategy.
        /// 
        /// <seealso cref="IDetermineOriginStrategy"/>
        /// </summary>
        /// <param name="globalPosition">The Vector3d which represents the global simulation position.</param>
        /// <returns>The Unity Vector3 which represents the corresponding unity position.</returns>
        public static UnityEngine.Vector3 RemapGlobalToUnityVector(this Improbable.Math.Vector3d globalPosition)
        {
            return CoordinateRemappingBehaviour.GlobalVectorToUnityPosition(globalPosition);
        }

        /// <summary>
        /// Converts a Unity local position to a global position represented by a Vector3d via
        /// coordinate remapping using the IDetermineOriginStrategy.
        /// 
        /// <seealso cref="IDetermineOriginStrategy"/>
        /// </summary>
        /// <param name="unityPosition">The Unity Vector3 which represents a local unity global position.</param>
        /// <returns>The Vector3d which represents the global simulation position.</returns>
        public static Improbable.Math.Vector3d RemapUnityVectorToGlobalVector(this UnityEngine.Vector3 unityPosition)
        {
            return CoordinateRemappingBehaviour.UnityPositionToGlobalVector(unityPosition);
        }

        /// <summary>
        /// Converts a Coordinate which represents a global position to a Unity local position via
        /// coordinate remapping using the IDetermineOriginStrategy.
        /// 
        /// <seealso cref="IDetermineOriginStrategy"/>
        /// </summary>
        /// <param name="globalPosition">The Coordinate which represents the global simulation position.</param>
        /// <returns>The Unity Vector3 which represents the corresponding unity position.</returns>
        public static UnityEngine.Vector3 RemapGlobalToUnityVector(this Improbable.Math.Coordinates globalPosition)
        {
            return CoordinateRemappingBehaviour.GlobalCoordinatesToUnityPosition(globalPosition);
        }

        /// <summary>
        /// Converts a Unity local position to a global position represented by a Coordinate via
        /// coordinate remapping using the IDetermineOriginStrategy.
        /// 
        /// <seealso cref="IDetermineOriginStrategy"/>
        /// </summary>
        /// <param name="unityPosition">The Unity Vector3 which represents a local unity global position.</param>
        /// <returns>The Coordinate which represents the global simulation position.</returns>
        public static Improbable.Math.Coordinates RemapUnityVectorToGlobalCoordinates(this UnityEngine.Vector3 unityPosition)
        {
            return CoordinateRemappingBehaviour.UnityPositionToGlobalCoordinates(unityPosition);
        }
    }
}