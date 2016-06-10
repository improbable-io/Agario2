using System;
using System.Collections.Generic;
using Assets.Improbable.Unity.Entity;
using Improbable.Unity.Entity;
using log4net;

namespace Improbable.Core.Entity
{
    //Universe is a lookup system from IDs to EntityGameObjects

    public class Universe : IUniverseInternal
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Universe));
        private readonly Dictionary<EntityId, IEntityObject> UniverseSet = new Dictionary<EntityId, IEntityObject>();

        public void IterateOverAllEntityObjects(Action<EntityId, IEntityObject> action)
        {
            foreach (var keyValue in UniverseSet)
            {
                action(keyValue.Key, keyValue.Value);
            }
        }

        public IEntityObject Get(EntityId entityId)
        {
            IEntityObject entity;
            UniverseSet.TryGetValue(entityId, out entity);
            return entity;
        }

        public bool ContainsEntity(EntityId entityId)
        {
            return UniverseSet.ContainsKey(entityId);
        }

        public void AddEntity(EntityId entityId, IEntityObject entity)
        {
            if (UniverseSet.ContainsKey(entityId))
            {
                Logger.ErrorFormat("Duplicate key {0} for object {1} with key {0}", entityId, entity.UnderlyingGameObject.name);
                return;
            }
            UniverseSet.Add(entityId, entity);
        }

        private void Destroy(IEntityObject entity)
        {
            if (entity != null)
            {
                entity.Destroy();
                UniverseSet.Remove(entity.EntityId);
            }
            else
            {
                Logger.Error("Trying to destory an null entity");
            }
        }

        public void Destroy(EntityId entityId)
        {
            Destroy(Get(entityId));
        }
    }
}