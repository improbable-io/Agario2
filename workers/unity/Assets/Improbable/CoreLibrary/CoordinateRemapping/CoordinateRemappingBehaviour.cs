using System;
using System.Collections.Generic;
using Improbable.CoreLibrary.CoordinateRemapping.DetermineOrigin;
using Improbable.CoreLibrary.CoordinateRemapping.MoveOrigin;
using Improbable.Math;
using Improbable.Unity.Common.Core.Math;
using log4net;
using UnityEngine;

namespace Improbable.CoreLibrary.CoordinateRemapping
{
    /// <summary>
    /// The CoordinateRemappingBehaviour class acts as the MonoBehaviour that should be added to the GameEntry object
    /// to enable Coordinate Remapping. If the Behaviour cannot find a IDetermineOriginStrategy or a
    /// IMoveOriginStrategy on the GameEntry object it will add default versions:
    ///   - EntityBoundsReactiveDetermineOriginStrategy
    ///   - TransformMoveOriginStrategy
    /// 
    /// These defaults can be overriden by extending this class or by simply adding the strategies on the 
    /// GameEntry object.
    /// 
    /// The CoordinateRemappingBehaviour class provides static functions for converting between Improbable Vector3d and
    /// Coordinates and Unity Vector3 via remapping. These functions are also available Implicitly through the
    /// CoordinateRemappingImplicits.
    /// 
    /// It is important to note that there can only be a single IDetermineOriginStrategy. However, there can
    /// be multiple IMoveOriginStrategies.
    /// 
    /// <seealso cref="IDetermineOriginStrategy"/>
    /// <seealso cref="IMoveOriginStrategy"/>
    /// <seealso cref="CoordinateRemappingImplicits"/>
    /// </summary>
    public class CoordinateRemappingBehaviour : MonoBehaviour
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CoordinateRemappingBehaviour));

        protected static IDetermineOriginStrategy DetermineOriginStrategy { get; private set; }
        protected static readonly IList<IMoveOriginStrategy> MoveOriginStrategies = new List<IMoveOriginStrategy>();

        protected virtual void Awake()
        {
            if (DetermineOriginStrategy != null)
            {
                throw new NotSupportedException("Only a single CoordinateRemmappingBehaviour can be present in scene.");
            }

            var determineOriginStrategies = GetComponents<IDetermineOriginStrategy>();
            if (determineOriginStrategies.Length == 0)
            {
                DetermineOriginStrategy = AddDefaultDetermineOriginStrategy();
            }
            else
            {
                if (determineOriginStrategies.Length > 1)
                {
                    Logger.WarnFormat("Multiple {0} detected. You may only have a single {0}. Only using the first, {1}.",
                        typeof(IDetermineOriginStrategy).Name,
                        determineOriginStrategies[0].GetType().Name);
                }
                DetermineOriginStrategy = determineOriginStrategies[0];
            }

            IList<IMoveOriginStrategy> moveOriginStrategies = GetComponents<IMoveOriginStrategy>();
            if (MoveOriginStrategies.Count == 0)
            {
                moveOriginStrategies = AddDefaultMoveOriginStrategies();
            }
            for (var index = 0; index < moveOriginStrategies.Count; index++)
            {
                MoveOriginStrategies.Add(moveOriginStrategies[index]);
            }


            DetermineOriginStrategy.OnOriginMoved = TriggerOriginChanged;
        }

        protected virtual IDetermineOriginStrategy AddDefaultDetermineOriginStrategy()
        {
            return gameObject.AddComponent<EntityBoundsReactiveDetermineOriginStrategy>();
        }

        protected virtual IList<IMoveOriginStrategy> AddDefaultMoveOriginStrategies()
        {
            return new List<IMoveOriginStrategy> {
                gameObject.AddComponent<TransformMoveOriginStrategy>()
            };
        }

        protected virtual void TriggerOriginChanged(Vector3d oldOrigin, Vector3d newOrigin)
        {
            for (var index = 0; index < MoveOriginStrategies.Count; index++)
            {
                MoveOriginStrategies[index].MoveOrigin(oldOrigin, newOrigin);
            }
            if (OnOriginChanged != null)
            {
                OnOriginChanged(oldOrigin, newOrigin);
            }
        }

        /// <summary>
        /// The OffsetOrigin which is currently being used. It is determined by the IDetermineOffsetStrategy
        /// 
        /// GlobalPosition - OffsetOrigin = LocalPosition
        /// 
        /// <seealso cref="IDetermineOriginStrategy"/>
        /// </summary>
        public static Vector3d OffsetOrigin
        {
            get
            {
                return DetermineOriginStrategy == null ? new Vector3d() : DetermineOriginStrategy.OffsetOrigin;
            }
        }

        public static event Action<Vector3d, Vector3d> OnOriginChanged;

        // Types are fully qualified here for clarity.

        /// <summary>
        /// Converts a Vector3d which represents a global position to a Unity local position via
        /// coordinate remapping using the IDetermineOriginStrategy.
        /// 
        /// <seealso cref="IDetermineOriginStrategy"/>
        /// </summary>
        /// <param name="globalPosition">The Vector3d which represents the global simulation position.</param>
        /// <returns>The Unity Vector3 which represents the corresponding unity position.</returns>
        public static UnityEngine.Vector3 GlobalVectorToUnityPosition(Improbable.Math.Vector3d globalPosition)
        {
            return DetermineOriginStrategy == null ? globalPosition.ToUnityVector() :
                DetermineOriginStrategy.GlobalPositionToUnityPosition(globalPosition);
        }

        /// <summary>
        /// Converts a Unity local position to a global position represented by a Vector3d via
        /// coordinate remapping using the IDetermineOriginStrategy.
        /// 
        /// <seealso cref="IDetermineOriginStrategy"/>
        /// </summary>
        /// <param name="localPosition">The Unity Vector3 which represents a local unity global position.</param>
        /// <returns>The Vector3d which represents the global simulation position.</returns>
        public static Improbable.Math.Vector3d UnityPositionToGlobalVector(UnityEngine.Vector3 localPosition)
        {
            return DetermineOriginStrategy == null ? localPosition.ToNativeVector() :
                DetermineOriginStrategy.UnityPositionToGlobalPosition(localPosition);
        }

        /// <summary>
        /// Converts a Coordinate which represents a global position to a Unity local position via
        /// coordinate remapping using the IDetermineOriginStrategy.
        /// 
        /// <seealso cref="IDetermineOriginStrategy"/>
        /// </summary>
        /// <param name="globalPosition">The Coordinate which represents the global simulation position.</param>
        /// <returns>The Unity Vector3 which represents the corresponding unity position.</returns>
        public static UnityEngine.Vector3 GlobalCoordinatesToUnityPosition(Improbable.Math.Coordinates globalPosition)
        {
            var vector = new Vector3d(globalPosition.X, globalPosition.Y, globalPosition.Z);
            return GlobalVectorToUnityPosition(vector);
        }

        /// <summary>
        /// Converts a Unity local position to a global position represented by a Coordinate via
        /// coordinate remapping using the IDetermineOriginStrategy.
        /// 
        /// <seealso cref="IDetermineOriginStrategy"/>
        /// </summary>
        /// <param name="localPosition">The Unity Vector3 which represents a local unity global position.</param>
        /// <returns>The Coordinate which represents the global simulation position.</returns>
        public static Improbable.Math.Coordinates UnityPositionToGlobalCoordinates(UnityEngine.Vector3 localPosition)
        {
            var vector = UnityPositionToGlobalVector(localPosition);
            return new Coordinates(vector.X, vector.Y, vector.Z);
        }
    }
}