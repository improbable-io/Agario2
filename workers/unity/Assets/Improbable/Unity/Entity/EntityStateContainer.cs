using System;
using System.Collections.Generic;
using Improbable.Core.Serialization;
using Improbable.Entity.State;
using log4net;

namespace Improbable.Unity.Common.Entity.State
{
    // TODO: matej: Move the entity state container to Core once the tests are moved.

    /// <summary>
    ///     Default implementation of IEntityStateContainer
    /// </summary>
    public class EntityStateContainer : IEntityStateContainer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EntityStateContainer));

        private readonly EntityId entityId;

        // this contains the concrete state type (e.g. DynamicSlots).  It implements IReadWriteEntityState, and the Writer version (which implements the Reader version)
        private readonly Dictionary<uint, IReadWriteEntityState> componentIdToStateMap = new Dictionary<uint, IReadWriteEntityState>();
        private readonly HashSet<uint> authoritativeStates = new HashSet<uint>();

        private readonly IStateSender updateStateSender;

        private readonly List<IEntityStateSubscriber> subscriberList = new List<IEntityStateSubscriber>();

        public EntityStateContainer(EntityId entityId, IStateSender updateStateSender)
        {
            this.entityId = entityId;
            this.updateStateSender = updateStateSender;
        }

        public bool HasAuthoritativeState()
        {
            return authoritativeStates.Count > 0;
        }

        public void AddSubscriber(IEntityStateSubscriber subscriber)
        {
            subscriberList.Add(subscriber);
        }

        public void RemoveSubscriber(IEntityStateSubscriber subscriber)
        {
            subscriberList.Remove(subscriber);
        }

        public void AddState(IEntityStateUpdate stateData)
        {
            var state = stateData.CreateState(entityId, updateStateSender);
            componentIdToStateMap.Add(stateData.ComponentId, state);
            OnStateAdded(state);
        }

        public void UpdateStateFromNetwork(IEntityStateUpdate stateData)
        {
            IReadWriteEntityState state;
            if (componentIdToStateMap.TryGetValue(stateData.ComponentId, out state))
            {
                state.UpdateFromNetwork(stateData);
            }
            else
            {
                LogErrorIfStateKnown(stateData.ComponentId, string.Format("Unable to update state {0} for entity {1}", stateData.ComponentId, entityId));
            }
        }

        public void RemoveState(uint componentId)
        {
            IReadWriteEntityState state;
            if (componentIdToStateMap.TryGetValue(componentId, out state))
            {
                OnStateRemoved(state);
                componentIdToStateMap.Remove(componentId);
                authoritativeStates.Remove(componentId);
            }
            else
            {
                LogErrorIfStateKnown(componentId, string.Format("Unable to remove state {0} from entity {1}", componentId, entityId));
            }
        }

        public void DelegateState(uint componentId)
        {
            IReadWriteEntityState state;
            if (componentIdToStateMap.TryGetValue(componentId, out state))
            {
                state.SetAuthoritativeHere(true);
                authoritativeStates.Add(componentId);
                OnStateDelegated(state);
            }
            else
            {
                LogErrorIfStateKnown(componentId, string.Format("Unable to delegate state {0} for entity {1}", componentId, entityId));
            }
        }

        public void UndelegateState(uint componentId)
        {
            IReadWriteEntityState state;
            if (componentIdToStateMap.TryGetValue(componentId, out state))
            {
                state.SetAuthoritativeHere(false);
                authoritativeStates.Remove(componentId);
                OnStateUndelegated(state);
            }
            else
            {
                LogErrorIfStateKnown(componentId, string.Format("Unable to undelegate state {0} for entity {1}", componentId, entityId));
            }
        }

        // ReSharper disable ForCanBeConvertedToForeach
        private void OnStateDelegated(IReadWriteEntityState state)
        {
            for (int index = 0; index < subscriberList.Count; ++index)
            {
                var stateSubscriber = subscriberList[index];
                stateSubscriber.OnStateDelegated(state);
            }
        }

        private void OnStateUndelegated(IReadWriteEntityState state)
        {
            for (int index = 0; index < subscriberList.Count; ++index)
            {
                var stateSubscriber = subscriberList[index];
                stateSubscriber.OnStateUndelegated(state);
            }
        }

        private void OnStateAdded(IReadWriteEntityState state)
        {
            for (int index = 0; index < subscriberList.Count; ++index)
            {
                var stateSubscriber = subscriberList[index];
                stateSubscriber.OnStateAdded(state);
            }
        }

        private void OnStateRemoved(IReadWriteEntityState state)
        {
            for (int index = 0; index < subscriberList.Count; ++index)
            {
                var stateSubscriber = subscriberList[index];
                stateSubscriber.OnStateRemoved(state);
            }
        }

        private static void LogErrorIfStateKnown(uint componentId, string message)
        {
            if (!UnknownComponentIdTracker.Instance.Contains(componentId))
            {
                Logger.Error(message + ", since the entity does not have this state.");
            }
        }

        // ReSharper restore ForCanBeConvertedToForeach
    }
}