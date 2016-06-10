using UnityEngine;
using System.Collections.Generic;
using Improbable.Corelib.Util;
using Improbable.Corelibrary.Transforms;
using Improbable.Corelibrary.Transforms.Offsets;
using Improbable.Unity.Visualizer;
using Transform = Improbable.Corelibrary.Transforms.Transform;
using Improbable.Util.Collections;

namespace Improbable.CoreLibrary.Transforms.Offsets
{
    /// <summary>
    /// The TransformOffsetsUpdater is used to update the TransformKeyOffsetsState. There are a variety of configuration parameters
    /// which control the rate of updates.
    /// </summary>
    public class TransformOffsetsUpdater : MonoBehaviour
    {
        [Require] protected TransformKeyOffsetsStateWriter TransformKeyOffsetsStateWriter;
        [Require] protected TransformHierarchyStateReader TransformHierarchyStateReader;

        protected readonly IDictionary<string, TransformInfo> TransformDictionary =
            new Dictionary<string, TransformInfo>();

        protected readonly HashSet<string> SubscribedList =
            new HashSet<string>(); 

        protected float LastUpdateTime = float.MinValue;

        protected TransformOffsetsRegistry Registry;
        protected UnityEngine.Transform CachedTransform;
        protected ITransformOffsetsUpdateSettings TransformOffsetsUpdateSettings; 

        protected virtual void Awake()
        {
            TransformOffsetsUpdateSettings = GetComponent<ITransformOffsetsUpdateSettings>();
            if (TransformOffsetsUpdateSettings == null)
            {
                throw new MissingComponentException(string.Format("{0} on {1} expects a {2}.", GetType().Name, gameObject.name, typeof(ITransformOffsetsUpdateSettings).Name));
            }
            CachedTransform = transform;
            Registry = GetComponent<TransformOffsetsRegistry>();
            if (Registry == null)
            {
                throw new MissingComponentException(string.Format("{0} on {1} expects a {2}.", GetType().Name, gameObject.name, typeof(TransformOffsetsRegistry).Name));
            }
            Registry.OnOffsetAdded += OnOffsetAdded;
            Registry.OnOffsetRemoved += OnOffsetRemoved;
        }

        protected virtual void OnEnable()
        {
            UpdateState();
            RegisterSubscriberListUpdater();
        }

        protected virtual void Update()
        {
            UpdateIfNecessary();
        }

        private void OnOffsetAdded(string key, UnityEngine.Transform offsetTransform)
        {
            var transformInfo = new TransformInfo(offsetTransform, CachedTransform);
            TransformDictionary.Add(key, transformInfo);
            UpdateState();
        }

        private void OnOffsetRemoved(string key)
        {
            TransformDictionary.Remove(key);
            UpdateState();
        }

        protected void UpdateIfNecessary()
        {
            if (!TimeExceedsThreshold())
            {
                return;
            }

            var shouldUpdate = false;
            foreach (var keyValue in TransformDictionary)
            {
                var offsetKey = keyValue.Key;
                var transformInfo = keyValue.Value;
                var needsUpdating = UpdateAndCheckIfTransformNeedsUpdating(offsetKey, transformInfo);
                shouldUpdate = needsUpdating || shouldUpdate;
            }
            if (shouldUpdate)
            {
                UpdateState();
            }
        }

        protected void UpdateState()
        {
            if (TransformKeyOffsetsStateWriter == null)
            {
                return;
            }
            var stateDictionary = new Dictionary<string, Transform>();
            foreach (var item in TransformDictionary)
            {
                var transformInfo = item.Value;
                stateDictionary.Add(item.Key, transformInfo.LastCalculatedTransform);
                transformInfo.LastUpdatedTransform = transformInfo.LastCalculatedTransform;
            }
            TransformKeyOffsetsStateWriter.Update.KeyMap(stateDictionary).FinishAndSend();
            LastUpdateTime = Time.time;
        }

        protected bool UpdateAndCheckIfTransformNeedsUpdating(string offsetKey, TransformInfo transformInfo)
        {
            if (TransformOffsetsUpdateSettings.GetOnlyUpdateWhenUsed() && !CurrentlySubscribed(offsetKey))
            {
                return false;
            }
            transformInfo.UpdateLastCalculatedTransform(CachedTransform);
            return PositionExceedsThreshold(transformInfo) || RotationExceedsThreshold(transformInfo);
        }

        protected void RegisterSubscriberListUpdater()
        {
            TransformHierarchyStateReader.ChildrenUpdated += UpdateSubscriberList;
        }

        private void UpdateSubscriberList(IReadOnlyList<Child> childrenList)
        {
            SubscribedList.Clear();
            for (var index = 0; index < childrenList.Count; index++)
            {
                SubscribedList.Add(childrenList[index].Key);
            }
        }

        protected bool CurrentlySubscribed(string offsetKey)
        {
            return SubscribedList.Contains(offsetKey);
        }

        protected virtual bool RotationExceedsThreshold(TransformInfo transformInfo)
        {
            var lastRotation = transformInfo.LastUpdatedTransform.Rotation;
            var newRotation = transformInfo.LastCalculatedTransform.Rotation;
            return Quaternion.Angle(lastRotation.ToUnityQuaternion(), newRotation.ToUnityQuaternion()) >=
                TransformOffsetsUpdateSettings.GetRotationNetworkUpdateAngleThreshold();
        }

        protected virtual bool PositionExceedsThreshold(TransformInfo transformInfo)
        {
            var lastPosition = transformInfo.LastUpdatedTransform.Position;
            var newPosition = transformInfo.LastCalculatedTransform.Position;
            return (newPosition - lastPosition).SquareMagnitude() >= TransformOffsetsUpdateSettings.GetPositionNetworkUpdateSquareDistanceThreshold();
        }

        protected virtual bool TimeExceedsThreshold()
        {
            return (Time.time - LastUpdateTime) > TransformOffsetsUpdateSettings.GetNetworkUpdatePeriodThresholdInSeconds();
        }

        protected class TransformInfo
        {
            public UnityEngine.Transform CachedTransformObject;
            public Transform LastUpdatedTransform;
            public Transform LastCalculatedTransform;

            public TransformInfo(UnityEngine.Transform offsetTransform, UnityEngine.Transform parentTransform)
            {
                CachedTransformObject = offsetTransform;
                UpdateLastCalculatedTransform(parentTransform);
            }

            public void UpdateLastCalculatedTransform(UnityEngine.Transform parentTransform)
            {
                LastCalculatedTransform = TransformOffsetsUtil.CalculateRelativeTransform(CachedTransformObject, parentTransform);
            }
        }
    }
}