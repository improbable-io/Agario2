using Improbable.Core.Entity;
using Improbable.Fapi.Protocol;

namespace Improbable.Unity.MessageProcessors
{
    public class StateUpdateMessageProcessor : EntityStateMessageProcessorBase<ToEngineComponentUpdate>
    {
        public StateUpdateMessageProcessor(IUniverse universe) : base(universe) { }

        protected override void ProcessMsg(ToEngineComponentUpdate componentUpdate)
        {
            var entityStateContainer = GetEntityStateContainer(componentUpdate.EntityId);
            entityStateContainer.UpdateStateFromNetwork(componentUpdate.StateUpdate);
            componentUpdate.ReturnToPool();
        }
    }
}