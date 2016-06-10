using Improbable.Corelib.Util;
using UnityEngine;
using Improbable.Corelibrary.Transforms;
using Improbable.Corelibrary.Transforms.Teleport;
using Improbable.CoreLibrary.CoordinateRemapping;
using Improbable.Math;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using Quaternion = Improbable.Corelib.Math.Quaternion;

namespace Improbable.CoreLibrary.Transforms.Local
{
    /// <summary>
    /// The LocalTransformTeleportBehaviour is used to handle Teleport Requests when the Unity worker is authoritative.
    /// 
    /// It is equivalent to the TransformTeleportBehaviour in Scala.
    /// </summary>
    public class LocalTransformTeleportBehaviour : MonoBehaviour
    {
        [Require] protected TransformStateWriter TransformStateWriter;
        [Require] protected TeleportAckStateWriter TeleportAckStateWriter;
        [Require] protected TeleportRequestStateReader TeleportRequestStateReader;

        protected LocalTransformUpdaterBehaviour CachedLocalTransformUpdaterBehaviour;

        protected virtual void Awake()
        {
            CachedLocalTransformUpdaterBehaviour = GetComponent<LocalTransformUpdaterBehaviour>();
        }

        protected virtual void OnEnable()
        {
            TeleportRequestStateReader.RequestUpdated += OnNewRequest;
        }

        protected virtual void OnDisable()
        {
            TeleportRequestStateReader.RequestUpdated -= OnNewRequest;
        }

        protected void OnNewRequest(int requestId)
        {
            if (ShouldHandleTeleportRequest(requestId))
            {
                HandleTeleportRequest();
            }
        }
        protected virtual void HandleTeleportRequest()
        {
            var newLocalPosition = TeleportRequestStateReader.LocalPosition ?? GetCurrentPosition(TeleportRequestStateReader.Parent.HasValue);
            var newLocalRotation = TeleportRequestStateReader.LocalRotation ?? GetCurrentRotation(TeleportRequestStateReader.Parent.HasValue);

            TransformStateWriter.Update
                .LocalPosition(newLocalPosition)
                .LocalRotation(newLocalRotation)
                .Parent(TeleportRequestStateReader.Parent)
                .FinishAndSend();

            TeleportAckStateWriter.Update
                .LastExecutedRequest(TeleportRequestStateReader.Request)
                .FinishAndSend();

            if (CachedLocalTransformUpdaterBehaviour != null)
            {
                CachedLocalTransformUpdaterBehaviour.ReInitializeTransformFromState();
            }
        }

        private Vector3d GetCurrentPosition(bool isLocal)
        {
            return isLocal ? transform.localPosition.ToNativeVector() : transform.position.RemapUnityVectorToGlobalVector();
        }

        private Quaternion GetCurrentRotation(bool isLocal)
        {
            return isLocal ? transform.localRotation.ToNativeQuaternion() : transform.rotation.ToNativeQuaternion();
        }

        protected bool ShouldHandleTeleportRequest(int requestId)
        {
            return requestId > TeleportAckStateWriter.LastExecutedRequest;
        }
    }
}