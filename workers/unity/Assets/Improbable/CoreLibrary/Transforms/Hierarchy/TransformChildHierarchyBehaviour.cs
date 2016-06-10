using System;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core.Entity;
using Improbable.Corelibrary.Transforms;
using Improbable.CoreLibrary.Transforms.Global;
using Improbable.CoreLibrary.Transforms.Local;
using Improbable.Unity.Visualizer;
using IoC;
using Transform = UnityEngine.Transform;

namespace Improbable.CoreLibrary.Transforms.Hierarchy
{
    /// <summary>
    /// The TransformChildHierarchyBehaviour attemps to find the parent object in the Unity scene of this object and uses the
    /// TransformParentHierarchyBehaviour to determine where the child should be slotted.
    /// 
    /// TransformParentHierarchyBehaviour tells the child where to slot by using: NotifyParentTransformOffsetUpdated
    /// 
    /// The TransformChildHierarchyBehaviour provides to events to listen to which allows other Behaviours to add and remove
    /// visualizers when this gameObject is parented and unparented: OnTransformParented and OnTransformUnparented.
    /// 
    /// <seealso cref="TransformParentHierarchyBehaviour"/>
    /// </summary>
    public class TransformChildHierarchyBehaviour : MonoBehaviour
    {
        [Require] protected TransformStateReader TransformStateReader;

        [Inject] protected IUniverse Universe;

        protected Transform CachedTransform;

        protected Transform OriginalParentTransform;
        protected TransformParentHierarchyBehaviour CurrentParentHierarchyBehaviour;

        protected Parent? currentParent;
        protected Parent? CurrentParent
        {
            get { return currentParent; }
            set
            {
                if (!currentParent.Equals(value))
                {
                    if (value.HasValue)
                    {
                        TriggerOnParentedCallbacks(value.Value);
                    }
                    else
                    {
                        TriggerOnUnparentedCallbacks();
                    }
                }
                currentParent = value;
            }
        }

        protected readonly List<Action<Parent>> onParentedCallbacks = new List<Action<Parent>>();
        protected readonly List<Action> onUnparentedCallbacks = new List<Action>();

        protected HierarchyMode currentMode = HierarchyMode.Initializing;
        protected HierarchyMode CurrentMode
        {
            get { return currentMode; }
            set
            {
                if (currentMode != value)
                {
                    ChangeVisualizersForMode(false, currentMode);
                    ChangeVisualizersForMode(true, value);
                }

                currentMode = value;
            }
        }

        public enum HierarchyMode
        {
            Initializing, Local, Global
        }

        public event Action<Parent> OnTransformParented
        {
            add
            {
                onParentedCallbacks.Add(value);
                if (CurrentParent.HasValue)
                {
                    value(CurrentParent.Value);
                }
            }
            remove { onParentedCallbacks.Remove(value); }
        }

        public event Action OnTransformUnparented
        {
            add
            {
                onUnparentedCallbacks.Add(value);
                if (!CurrentParent.HasValue)
                {
                    value();
                }
            }
            remove { onUnparentedCallbacks.Remove(value); }
        }

        protected readonly List<object> GlobalModeVisualizers = new List<object>();
        protected readonly List<object> LocalModeVisualizers = new List<object>();

        protected virtual void Awake()
        {
            CachedTransform = transform;
            OriginalParentTransform = CachedTransform.parent;

            var globalVisualizers = GetComponents<IGlobalModeBehaviour>();
            for (var index = 0; index < globalVisualizers.Length; index++)
            {
                GlobalModeVisualizers.Add(globalVisualizers[index]);
            }

            var localVisualizers = GetComponents<ILocalModeBehaviour>();
            for (var index = 0; index < localVisualizers.Length; index++)
            {
                LocalModeVisualizers.Add(localVisualizers[index]);
            }
        }

        protected virtual void OnEnable()
        {
            // GlobalTransformFallback starts disabled
            ResetCurrentParent();
            CurrentMode = HierarchyMode.Initializing;
            TransformStateReader.ParentUpdated += OnParentUpdated;
        }

        protected virtual void OnDisable()
        {
            TransformStateReader.ParentUpdated -= OnParentUpdated;
            ResetCurrentParent();
            CurrentMode = HierarchyMode.Initializing;
        }

        public HierarchyMode GetCurrentMode()
        {
            return CurrentMode;
        }

        public void NotifyParentExists(EntityId parentId, TransformParentHierarchyBehaviour hierarchyBehaviour)
        {
            if (CurrentParent.HasValue)
            {
                return;
            }
            if (TransformStateReader.Parent.HasValue && parentId.Equals(TransformStateReader.Parent.Value.ParentId))
            {
                TrySetNewParent(TransformStateReader.Parent.Value, hierarchyBehaviour);
            }
        }

        public void NotifyParentDisabled()
        {
            if (TransformStateReader != null)
            {
                OnParentUpdated(TransformStateReader.Parent);
            }
            else
            {
                ResetCurrentParent();
            }
        }

        public void NotifyParentTransformOffsetUpdated(Transform transformOffset)
        {
            if (!GameObjectIsEnabled())
            {
                return;
            }

            CachedTransform.parent = transformOffset;
        }

        protected void TriggerOnParentedCallbacks(Parent parent)
        {
            for (var index = 0; index < onParentedCallbacks.Count; index++)
            {
                onParentedCallbacks[index](parent);
            }
        }

        protected void TriggerOnUnparentedCallbacks()
        {
            for (var index = 0; index < onUnparentedCallbacks.Count; index++)
            {
                onUnparentedCallbacks[index]();
            }
        }

        protected void ChangeVisualizersForMode(bool enable, HierarchyMode mode)
        {
            if (!GameObjectIsEnabled())
            {
                return;
            }

            var entityObject = gameObject.GetEntityObject();
            if (entityObject == null)
            {
                return;
            }

            var visualizers = GetVisualizersForMode(mode);
            if (visualizers == null)
            {
                return;
            }

            if (enable)
            {
                entityObject.TryEnableVisualizers(visualizers);
            }
            else
            {
                entityObject.DisableVisualizers(visualizers);
            }
        }

        protected List<object> GetVisualizersForMode(HierarchyMode mode)
        {
            switch (mode)
            {
                case HierarchyMode.Global:
                    return GlobalModeVisualizers;
                case HierarchyMode.Local:
                    return LocalModeVisualizers;
                default:
                    return null;
            }
        }

        protected void OnParentUpdated(Parent? newParent)
        {
            ResetCurrentParent();
            if (!newParent.HasValue)
            {
                SetNoParent();
            }
            else
            {
                TrySetNewParent(newParent.Value);
            }
        }

        protected void SetNoParent()
        {
            CurrentMode = HierarchyMode.Local;
        }

        protected void ResetCurrentParent()
        {
            if (!GameObjectIsEnabled())
            {
                return;
            }

            if (CurrentParent != null)
            {
                CachedTransform.parent = OriginalParentTransform;
                if (CurrentParentHierarchyBehaviour != null)
                {
                    CurrentParentHierarchyBehaviour.UnregisterTransformChild(CurrentParent.Value.Key, this);
                }
                CurrentParent = null;
                CurrentParentHierarchyBehaviour = null;
            }
        }

        protected void TrySetNewParent(Parent newParent, TransformParentHierarchyBehaviour transformParentHierarchyBehaviour = null)
        {
            if (transformParentHierarchyBehaviour == null && !TryGetParentHierarchyVisualizer(newParent.ParentId, out transformParentHierarchyBehaviour))
            {
                CurrentMode = HierarchyMode.Global;
                return;
            }
            CurrentParentHierarchyBehaviour = transformParentHierarchyBehaviour;

            CurrentParent = newParent;
            CurrentParentHierarchyBehaviour.RegisterTransformChild(newParent.Key, this);
            CurrentMode = HierarchyMode.Local;
        }

        protected bool TryGetParentHierarchyVisualizer(EntityId parentId, out TransformParentHierarchyBehaviour transformParentHierarchyBehaviour)
        {
            transformParentHierarchyBehaviour = null;
            var parentEntityObject = Universe.Get(parentId);
            if (parentEntityObject == null)
            {
                return false;
            }

            var parentGameObject = parentEntityObject.UnderlyingGameObject;
            if (parentGameObject == null)
            {
                return false;
            }

            transformParentHierarchyBehaviour = parentGameObject.GetComponent<TransformParentHierarchyBehaviour>();
            if (transformParentHierarchyBehaviour == null || !transformParentHierarchyBehaviour.isActiveAndEnabled)
            {
                return false;
            }

            return true;
        }

        protected bool GameObjectIsEnabled()
        {
            return gameObject != null && gameObject.activeInHierarchy;
        }
    }
}