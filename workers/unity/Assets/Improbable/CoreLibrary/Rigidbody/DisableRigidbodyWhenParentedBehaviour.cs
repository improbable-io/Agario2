using System;
using System.Collections.Generic;
using Improbable.Core.GameLogic.Visualizers;
using Improbable.CoreLibrary.Transforms.Hierarchy;

namespace Improbable.CoreLibrary.Rigidbody
{
    /// <summary>
    /// The DisableRigidbodyWhenParentedBehaviour disables the RigidbodyVisualizer when the transform is parented.
    /// 
    /// <seealso cref="RigidbodyVisualizer"/>
    /// <seealso cref="AbstractDisableWhenParentedBehaviour"/>
    /// </summary>
    public class DisableRigidbodyWhenParentedBehaviour : AbstractDisableWhenParentedBehaviour
    {
        protected override void PopulateBehavioursToDisable(Action<Type> addBehaviourToDisable)
        {
            addBehaviourToDisable(typeof(RigidbodyVisualizer));
        }
    }
}