using Improbable.Core.Entity;
using Improbable.Fapi.Protocol;

namespace Improbable.Unity.MessageProcessors
{
    internal class DelegateStateMessageProcessor : EntityStateMessageProcessorBase<DelegateComponent>
    {
        public DelegateStateMessageProcessor(IUniverse universe) : base(universe) { }

        protected override void ProcessMsg(DelegateComponent delegateComponent)
        {
            var entityStateContainer = GetEntityStateContainer(delegateComponent.EntityId);
            entityStateContainer.UpdateStateFromNetwork(delegateComponent.CanonicalComponent);
            entityStateContainer.DelegateState(delegateComponent.ComponentId);
            delegateComponent.ReturnToPool();
        }
    }
}