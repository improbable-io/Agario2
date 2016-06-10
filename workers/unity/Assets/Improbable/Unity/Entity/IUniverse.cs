using System;
using System.Collections;
using System.Collections.Generic;
using Improbable.Unity.Entity;

namespace Improbable.Core.Entity
{
    /// <summary>
    /// Contains all of the entities currently in the game
    /// </summary>
    /// <remarks>This is really just a dictionary with a destroy method?</remarks>
    public interface IUniverse
    {
        bool ContainsEntity(EntityId entityId);
        IEntityObject Get(EntityId entityId);
        void IterateOverAllEntityObjects(Action<EntityId, IEntityObject> action);
    }
}