using Improbable.Core.Entity;
using Improbable.Fapi.Protocol;

namespace Improbable.Unity.MessageProcessors
{
    internal class AddStateMessageProcessor : EntityStateMessageProcessorBase<AddComponent> 
    {
        public AddStateMessageProcessor(IUniverse universe) : base(universe) { }

        protected override void ProcessMsg(AddComponent addComponent)
        {
            var entityStateUpdate = addComponent.InitialData;
            var entityStateContainer = GetEntityStateContainer(entityStateUpdate.EntityId);
            entityStateContainer.AddState(entityStateUpdate);
            addComponent.ReturnToPool();
        }
    }
}