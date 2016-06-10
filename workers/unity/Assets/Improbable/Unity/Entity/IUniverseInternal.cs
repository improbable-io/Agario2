using Improbable;
using Improbable.Core.Entity;
using Improbable.Unity.Entity;

namespace Assets.Improbable.Unity.Entity
{
    internal interface IUniverseInternal : IUniverse
    {
        void AddEntity(EntityId entityId, IEntityObject entity);
        void Destroy(EntityId entityId);
    }
}
