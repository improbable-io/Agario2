using Improbable.Core.Entity;
using Improbable.Fapi.Protocol;

namespace Improbable.Unity.MessageProcessors
{
    public class RemoveStateMessageProcessor : EntityStateMessageProcessorBase<RemoveComponent>
    {
        public RemoveStateMessageProcessor(IUniverse universe) : base(universe) { }

        protected override void ProcessMsg(RemoveComponent removeComponent)
        {
            var entityStateContainer = GetEntityStateContainer(removeComponent.EntityId);
            entityStateContainer.RemoveState(removeComponent.ComponentId);

            removeComponent.ReturnToPool();
        }
    }
}