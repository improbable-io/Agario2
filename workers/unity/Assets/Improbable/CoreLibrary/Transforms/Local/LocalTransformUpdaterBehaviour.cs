using Assets.Improbable.Core.Physical;
using Improbable.Corelib.Physical.Visualizers;
using Improbable.Corelib.Util;
using Improbable.Corelibrary.Transforms;
using Improbable.CoreLibrary.CoordinateRemapping;
using Improbable.CoreLibrary.Transforms.Hierarchy;
using Improbable.Math;
using Improbable.Unity.Common.Core.Math;
using UnityEngine;
using Improbable.Unity.Visualizer;
using log4net;

namespace Improbable.CoreLibrary.Transforms.Local
{
    /// <summary>
    /// The LocalTransformUpdaterBehaviour updates the TransformState.
    ///
    /// The updater will only update the state after the Network threshold is met and either the position or
    /// rotation threshold is met.
    ///
    /// <seealso cref="ILocalModeBehaviour"/>
    /// </summary>
    [DontAutoEnable]
    public class LocalTransformUpdaterBehaviour : MonoBehaviour, ILocalModeBehaviour
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LocalTransformUpdaterBehaviour));
        private bool hasCenterOfMassErrorBeenLogged;

        [Require] protected TransformStateWriter TransformStateWriter;

        protected UnityEngine.Transform CachedTransform { get; private set; }
        protected IRigidbodyVisualizer CachedRigidbodyVisualizer { get; private set; }
        protected ThresholdBasedUpdateNotifier<TransformData> ThresholdBasedUpdateNotifier;
        protected TransformChildHierarchyBehaviour CachedTransformChildHierarchyBehaviour;
        protected IUpdateTransformSettings UpdateTransformSettings;

        protected virtual void Awake()
        {
            UpdateTransformSettings = GetComponent<IUpdateTransformSettings>();
            if (UpdateTransformSettings == null)
            {
                throw new MissingComponentException(string.Format("{0} on {1} expects a {2}.", GetType().Name, gameObject.name, typeof(IUpdateTransformSettings).Name));
            }
            CachedTransform = transform;
            CachedRigidbodyVisualizer = GetComponent<IRigidbodyVisualizer>();
            CachedTransformChildHierarchyBehaviour = GetComponent<TransformChildHierarchyBehaviour>();
        }

        protected virtual void OnEnable()
        {
            InitalizeTransform();
            SetupThresholdBasedUpdateNotifier();
        }

        protected virtual void FixedUpdate()
        {
            if (IsRigidbodySleeping())
            {
                return;
            }

            ThresholdBasedUpdateNotifier.RegisterNewValue(Time.time, GetLatestValue());
        }

        public void ReInitializeTransformFromState()
        {
            if (TransformStateWriter == null)
            {
                return;
            }

            InitalizeTransform();
            ThresholdBasedUpdateNotifier.Reset(Time.time, GetLatestValue());
        }

        protected virtual void InitalizeTransform()
        {
            var localPosition = TransformStateWriter.LocalPosition;
            var localRotation = TransformStateWriter.LocalRotation;

            if (HasParent())
            {
                transform.localPosition = localPosition.ToUnityVector();
                transform.localRotation = localRotation.ToUnityQuaternion();
            }
            else
            {
                transform.position = localPosition.RemapGlobalToUnityVector();
                transform.rotation = localRotation.ToUnityQuaternion();
            }
        }

        protected bool HasParent()
        {
            return TransformStateWriter.Parent.HasValue;
        }

        private void SetupThresholdBasedUpdateNotifier()
        {
            ThresholdBasedUpdateNotifier = new ThresholdBasedUpdateNotifier<TransformData>(UpdateTransformSettings.GetNetworkUpdatePeriodThreshold(), IsPastThreshold, Time.time, GetLatestValue());
            ThresholdBasedUpdateNotifier.ShouldUpdate += UpdateState;
        }

        protected virtual void UpdateState(float deltaTime, TransformData newValue)
        {
            if (!IsSafeToUpdate())
            {
                // This might happen if we are switching parents and this MonoBehaviour has not been disabled yet.
                // (There is a 1 frame delay).
                return;
            }
            TransformStateWriter.Update
                .LocalPosition(newValue.Position)
                .LocalRotation(newValue.Rotation)
                .Pivot(newValue.Pivot)
                .Timestamp(TransformStateWriter.Timestamp + deltaTime)
                .FinishAndSend();
        }

        protected TransformData GetLatestValue()
        {
            Vector3d position;
            Corelib.Math.Quaternion rotation;
            if (HasParent())
            {
                position = CachedTransform.localPosition.ToNativeVector();
                rotation = CachedTransform.localRotation.ToNativeQuaternion();
            }
            else
            {
                position = CachedTransform.position.RemapUnityVectorToGlobalVector();
                rotation = CachedTransform.rotation.ToNativeQuaternion();
            }
            var pivot = GetPivot();
            return new TransformData(position, rotation, pivot);
        }

        protected Vector3d GetPivot()
        {
            if (UpdateTransformSettings.GetUseCenterOfMassPivot())
            {
                var cachedRigidbody = GetRigidbody();
                if (cachedRigidbody != null)
                {
                    return cachedRigidbody.centerOfMass.ToNativeVector();
                }
                if (!hasCenterOfMassErrorBeenLogged)
                {
                    hasCenterOfMassErrorBeenLogged = true;
                    Logger.WarnFormat(
                        "Center of Mass Pivot was requested, but no Rigidbody or IRigidbodyVisualizer was present for GameObject {0}.",
                        gameObject.name);
                }
            }
            return UpdateTransformSettings.GetPivotOffset().ToNativeVector();
        }

        protected virtual bool IsPastThreshold(TransformData lastValue, TransformData newValue)
        {
            return IsPastDistanceThreshold(lastValue, newValue) || IsPastRotationThreshold(lastValue, newValue);
        }

        protected virtual bool IsPastRotationThreshold(TransformData lastValue, TransformData newValue)
        {
            return Quaternion.Angle(lastValue.Rotation.ToUnityQuaternion(), newValue.Rotation.ToUnityQuaternion()) >=
                   UpdateTransformSettings.GetRotationNetworkUpdateAngleThreshold();
        }

        protected virtual bool IsPastDistanceThreshold(TransformData lastValue, TransformData newValue)
        {
            return lastValue.Position.SquareDistance(newValue.Position) >= UpdateTransformSettings.GetPositionNetworkUpdateSquareDistanceThreshold();
        }

        protected UnityEngine.Rigidbody GetRigidbody()
        {
            if (CachedRigidbodyVisualizer == null)
            {
                return null;
            }
            return CachedRigidbodyVisualizer.Rigidbody;
        }

        protected bool IsRigidbodySleeping()
        {
            var cachedRigidbody = GetRigidbody();
            return cachedRigidbody != null && cachedRigidbody.IsSleeping();
        }

        protected bool IsSafeToUpdate()
        {
            if (CachedTransformChildHierarchyBehaviour == null)
            {
                return true;
            }
            return CachedTransformChildHierarchyBehaviour.GetCurrentMode() ==
                   TransformChildHierarchyBehaviour.HierarchyMode.Local;
        }
    }
}