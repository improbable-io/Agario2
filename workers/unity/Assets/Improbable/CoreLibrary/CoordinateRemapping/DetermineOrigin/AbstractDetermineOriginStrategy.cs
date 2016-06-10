using System;
using Improbable.Math;
using Improbable.Unity.Common.Core.Math;
using log4net;
using UnityEngine;

namespace Improbable.CoreLibrary.CoordinateRemapping.DetermineOrigin
{
    /// <summary>
    /// This is the default implementation of the IDetermineOriginStrategy and should be used for the majority
    /// of DetermineOriginStrategy implementations.
    /// 
    /// <seealso cref="IDetermineOriginStrategy"/>
    /// </summary>
    public abstract class AbstractDetermineOriginStrategy : MonoBehaviour, IDetermineOriginStrategy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AbstractDetermineOriginStrategy));

        private Action<Vector3d, Vector3d> onOriginMoved;
        public Action<Vector3d, Vector3d> OnOriginMoved
        {
            get { return onOriginMoved; }
            set
            {
                if (onOriginMoved != null)
                {
                    Logger.ErrorFormat("An OnOriginMoved action is already registered. " +
                                       "Please use the CoordinateRemappingBehaviour to register OnOriginMoved events instead.");
                    return;
                }
                onOriginMoved = value;
            }
        }

        private Vector3d offsetOrigin;
        public Vector3d OffsetOrigin
        {
            get { return offsetOrigin; }
            protected set
            {
                var oldOrigin = offsetOrigin;
                offsetOrigin = value;
                if (OnOriginMoved != null)
                {
                    OnOriginMoved(oldOrigin, value);
                }
            }
        }

        public virtual Vector3 GlobalPositionToUnityPosition(Vector3d globalPosition)
        {
            return (globalPosition - OffsetOrigin).ToUnityVector();
        }

        public virtual Vector3d UnityPositionToGlobalPosition(Vector3 unityPosition)
        {
            return unityPosition.ToNativeVector() + OffsetOrigin;
        }
    }
}