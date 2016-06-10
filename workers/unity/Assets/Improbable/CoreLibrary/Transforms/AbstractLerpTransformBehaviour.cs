using System;
using Improbable.Corelib.Util;
using Improbable.Math;
using UnityEngine;
using Quaternion = Improbable.Corelib.Math.Quaternion;

namespace Improbable.CoreLibrary.Transforms
{
    /// <summary>
    /// The AbstractLerpTransformBehaviour uses the TransformLinearInterpolator to interpolate between Transform positions
    /// which is provided by the RegisterPropertyListener method and GetCurrentStateData.
    /// 
    /// It is independent of any state. By default interpolation is only enabled when the state is non-authoritative.
    /// 
    /// The state, should contain a Timestamp which represents the time interval between the updates on the corresponding
    /// authoritative engine.
    /// </summary>
    abstract public class AbstractLerpTransformBehaviour : MonoBehaviour
    {
        protected abstract TransformData GetCurrentStateData();
        protected abstract float GetCurrentStateTimestamp();
        protected abstract bool IsAuthoritative();

        protected abstract void RegisterPropertyListener(Action action);
        protected abstract void UnregisterPropertyListener(Action action);
        protected abstract void RegisterAuthorityListener(Action<bool> action);
        protected abstract void UnregisterAuthorityListener(Action<bool> action);

        protected virtual void RegisterOtherListeners() { }
        protected virtual void UnregisterOtherListeners() { }

        protected abstract Vector3d CurrentPosition();
        protected abstract Quaternion CurrentRotation();
        protected abstract void SetPosition(Vector3d newPosition);
        protected abstract void SetRotation(Quaternion newRotation);
        
        protected Transform CachedTransform { get; private set; }
        protected TransformLinearInterpolator TransformLinearInterpolator { get; private set; }
        protected ILerpTransformSettings LerpTransformSettings;

        protected float LastUpdateTime;

        protected virtual void Awake()
        {
            LerpTransformSettings = GetComponent<ILerpTransformSettings>();
            if (LerpTransformSettings == null)
            {
                throw new MissingComponentException(string.Format("{0} on {1} expects a {2}.", GetType().Name, gameObject.name, typeof(ILerpTransformSettings).Name));
            }
            CachedTransform = transform;
            TransformLinearInterpolator = new TransformLinearInterpolator(LerpTransformSettings.GetInterpolationDelayInSeconds());
        }

        protected virtual void OnEnable()
        {
            RegisterAuthorityListener(OnAuthorityChanged);
            RegisterOtherListeners();
        }

        protected virtual void OnDisable()
        {
            UnregisterAuthorityListener(OnAuthorityChanged);
            UnregisterPropertyListener(OnTransformUpdated);
            UnregisterOtherListeners();
        }

        protected virtual void DoUpdate()
        {
            if (IsAuthoritative() || !TimeExceedsThreshold())
            {
                return;
            }

            var targetTransform = TransformLinearInterpolator.GetInterpolatedValue(Time.deltaTime);
            var position = targetTransform.Position;
            var rotation = targetTransform.Rotation;

            if (!PositionExceedsThreshold(position) && !RotationExceedsThreshold(rotation))
            {
                return;
            }

            SetPosition(position);
            SetRotation(rotation);

            LastUpdateTime = Time.time;
        }

        protected virtual void OnTransformUpdated()
        {
            TransformLinearInterpolator.AddValue(GetCurrentStateData(), GetCurrentStateTimestamp());
        }

        protected virtual void OnAuthorityChanged(bool isAuthoritativeHere)
        {
            if (isAuthoritativeHere)
            {
                UnregisterPropertyListener(OnTransformUpdated);
            }
            else
            {
                Reset();
                RegisterPropertyListener(OnTransformUpdated);
            }
        }

        protected virtual bool RotationExceedsThreshold(Quaternion rotation)
        {
            return UnityEngine.Quaternion.Angle(rotation.ToUnityQuaternion(), CurrentRotation().ToUnityQuaternion()) >=
                   LerpTransformSettings.GetMinAngleToInterpolateBetween();
        }

        protected virtual bool PositionExceedsThreshold(Vector3d position)
        {
            return position.SquareDistance(CurrentPosition()) >= LerpTransformSettings.GetMinDistanceToInterpolateBetween();
        }

        protected virtual bool TimeExceedsThreshold()
        {
            return (Time.time - LastUpdateTime) >= LerpTransformSettings.GetMaxSecondsToInterpolateAfterLastUpdate();
        }

        protected virtual void Reset()
        {
            var transformData = GetCurrentStateData();

            SetPosition(transformData.Position);
            SetRotation(transformData.Rotation);
            LastUpdateTime = Time.time;

            TransformLinearInterpolator.Reset(transformData, GetCurrentStateTimestamp());
        }
    }
}

