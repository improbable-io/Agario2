using System;
using UnityEngine;
using System.Collections.Generic;
using Improbable.Corelibrary.Transforms;
using log4net;

namespace Improbable.CoreLibrary.Transforms.Hierarchy
{
    /// <summary>
    /// The AbstractDisableWhenParentedBehaviour disables MonoBehaviours when the gameObject is parented.
    /// </summary>
    public abstract class AbstractDisableWhenParentedBehaviour : MonoBehaviour
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AbstractDisableWhenParentedBehaviour));

        protected TransformChildHierarchyBehaviour TransformChildHierarchyBehaviour;
        protected readonly IList<object> BehavioursToDisable = new List<object>();

        protected bool hasBeenDisabled;

        protected abstract void PopulateBehavioursToDisable(Action<Type> addBehaviourToDisable);

        protected virtual void Awake()
        {
            TransformChildHierarchyBehaviour = GetComponent<TransformChildHierarchyBehaviour>();
            if (TransformChildHierarchyBehaviour == null)
            {
                throw new MissingComponentException(string.Format("{0} expects a {1}.", GetType().Name, typeof(TransformChildHierarchyBehaviour).Name));
            }

            PopulateBehavioursToDisable(AddBehaviourToDisable);

            TransformChildHierarchyBehaviour.OnTransformParented += DisableParentBehaviours;
            TransformChildHierarchyBehaviour.OnTransformUnparented += EnableParentBehaviours;
        }

        protected virtual void OnEnable()
        {
            hasBeenDisabled = false;
        }

        private void AddBehaviourToDisable(Type componentType)
        {
            var behaviour = gameObject.GetComponent(componentType);
            if (behaviour == null)
            {
                throw new MissingComponentException(string.Format("{0} could not find {1} to disable.", GetType().Name, componentType.Name));
            }
            BehavioursToDisable.Add(behaviour);
        }

        private void DisableParentBehaviours(Parent parent)
        {
            var entityObject = gameObject.GetEntityObject();
            if (entityObject != null)
            {
                entityObject.DisableVisualizers(BehavioursToDisable);
                hasBeenDisabled = true;
            }
            else
            {
                Logger.WarnFormat("The {0} on gameObject {1} tried to disable behaviours when it was not an entity.", GetType().Name, gameObject.name);
            }
        }

        private void EnableParentBehaviours()
        {
            var entityObject = gameObject.GetEntityObject();
            if (entityObject != null && hasBeenDisabled)
            {
                entityObject.TryEnableVisualizers(BehavioursToDisable);
                hasBeenDisabled = false;
            }
        }
    }
}
