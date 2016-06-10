using System;
using UnityEngine;
using Improbable.Math;

namespace Improbable.CoreLibrary.CoordinateRemapping.DetermineOrigin
{
    /// <summary>
    /// An IDetermineOriginStrategy is used by the CoordinateRemappingBehaviour to determine the method
    /// for mapping between a Global Simulation Position and a Local Unity Global Position.
    /// 
    /// <seealso cref="CoordinateRemappingBehaviour"/>
    /// </summary>
    public interface IDetermineOriginStrategy
    {
        /// <summary>
        /// Maps a Global Simulation Position to a Local Unity Global Position.
        /// </summary>
        /// <param name="globalPosition">The global simulation position.</param>
        /// <returns>The local unity global position.</returns>
        Vector3 GlobalPositionToUnityPosition(Vector3d globalPosition);

        /// <summary>
        /// Maps a Local Unity Global Position to a Global Simulation Position.
        /// </summary>
        /// <param name="unityPosition">The local unity global position.</param>
        /// <returns>The global simulation position.</returns>
        Vector3d UnityPositionToGlobalPosition(Vector3 unityPosition);

        /// <summary>
        /// The current offset origin.
        /// 
        /// GlobalSimulationPosition - OffsetOrigin = LocalUnityPosition
        /// </summary>
        Vector3d OffsetOrigin { get; }

        /// <summary>
        /// This action should be fired when the Origin moves.
        /// </summary>
        Action<Vector3d, Vector3d> OnOriginMoved { get; set; }
    }
}