using Improbable.Core.Entity;
using Improbable.Entity.State;
using Improbable.Fapi.Protocol;

namespace Improbable.Unity.MessageProcessors
{
    public class UndelegateStateMessageProcessor : EntityStateMessageProcessorBase<UndelegateComponent>
    {
        public UndelegateStateMessageProcessor(IUniverse universe) : base(universe) {}

        protected override void ProcessMsg(UndelegateComponent undelegateComponent)
        {
            IEntityStateContainer entityStateContainer = GetEntityStateContainer(undelegateComponent.EntityId);
            entityStateContainer.UndelegateState(undelegateComponent.ComponentId);
            entityStateContainer.UpdateStateFromNetwork(undelegateComponent.CanonicalComponent);
            undelegateComponent.ReturnToPool();
        }
    }
}