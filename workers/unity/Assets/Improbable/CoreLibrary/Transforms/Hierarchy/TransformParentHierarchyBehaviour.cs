using System.Collections.Generic;
using Improbable.Core.Entity;
using Improbable.Corelibrary.Transforms;
using Improbable.CoreLibrary.Transforms.Offsets;
using Improbable.Unity.Visualizer;
using Improbable.Util.Collections;
using IoC;
using log4net;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace Improbable.CoreLibrary.Transforms.Hierarchy
{
    /// <summary>
    /// The TransformParentHierarchyBehaviour is accessed by the TransformChildHierarchyBehaviour to determine where
    /// the offset positions are.
    ///
    /// So that the TransformChildHierarchyBehaviour does not need to iterate every frame. This behaviour also contacts
    /// attempts to contact all the children OnEnable to notify them that the parent is available.
    ///
    /// <seealso cref="TransformChildHierarchyBehaviour"/>
    /// </summary>
    public class TransformParentHierarchyBehaviour : MonoBehaviour
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TransformParentHierarchyBehaviour));

        [Require] protected TransformHierarchyStateReader TransformHierarchyStateReader;

        [Inject] protected IUniverse Universe;

        protected TransformOffsetsRegistry Registry;
        protected Transform CachedTransform;

        protected readonly IDictionary<string, IList<TransformChildHierarchyBehaviour>> ChildrenVisualizers = new Dictionary<string, IList<TransformChildHierarchyBehaviour>>();

        protected bool isBeingDisabled = false;

        protected virtual void Awake()
        {
            CachedTransform = transform;
            Registry = GetComponent<TransformOffsetsRegistry>();
            if (Registry == null)
            {
                throw new MissingComponentException(string.Format("{0} expects a {1}.", GetType().Name, typeof(TransformOffsetsRegistry).Name));
            }
            Registry.OnOffsetAdded += NotifyChildrenOffsetAdded;
            Registry.OnOffsetRemoved += NotifyChildrenOffsetRemoved;
        }

        protected virtual void OnEnable()
        {
            isBeingDisabled = false;
            if (ChildrenVisualizers.Count > 0)
            {
                Logger.WarnFormat("Transform Parent Hierarchy Visualizer was not cleaned up correctly. There were still {0} children registered.", ChildrenVisualizers.Count);
                ChildrenVisualizers.Clear();
            }
            TransformHierarchyStateReader.ChildrenUpdated += FindAndNotifyAllChildrenParentExists;
        }

        protected virtual void OnDisable()
        {
            isBeingDisabled = true;
            NotifyAllChildrenParentDisabled();
            ChildrenVisualizers.Clear();
        }

        public Transform GetTransformOffset(string key)
        {
            Transform transformOffset;
            if (!Registry.TaggedOffsets.TryGetValue(key, out transformOffset))
            {
                transformOffset = transform;
            }
            return transformOffset;
        }

        public void RegisterTransformChild(string key, TransformChildHierarchyBehaviour hierarchyBehaviour)
        {
            AddChildVisualizer(key, hierarchyBehaviour);
            hierarchyBehaviour.NotifyParentTransformOffsetUpdated(GetTransformOffset(key));
        }

        public void UnregisterTransformChild(string key, TransformChildHierarchyBehaviour hierarchyBehaviour)
        {
            RemoveChildVisualizer(key, hierarchyBehaviour);
        }

        protected void NotifyChildrenOffsetRemoved(string key)
        {
            IList<TransformChildHierarchyBehaviour> listOfChildren;
            if (!ChildrenVisualizers.TryGetValue(key, out listOfChildren))
            {
                return;
            }
            for (var index = 0; index < listOfChildren.Count; index++)
            {
                var childHierarchyBehaviour = listOfChildren[index];
                if (childHierarchyBehaviour != null && childHierarchyBehaviour.isActiveAndEnabled)
                {
                    childHierarchyBehaviour.NotifyParentTransformOffsetUpdated(CachedTransform);
                }
            }
        }

        protected void NotifyChildrenOffsetAdded(string key, Transform offsetTransform)
        {
            IList<TransformChildHierarchyBehaviour> listOfChildren;
            if (!ChildrenVisualizers.TryGetValue(key, out listOfChildren))
            {
                return;
            }
            for (var index = 0; index < listOfChildren.Count; index++)
            {
                var childHierarchyBehaviour = listOfChildren[index];
                if (childHierarchyBehaviour != null && childHierarchyBehaviour.isActiveAndEnabled)
                {
                    childHierarchyBehaviour.NotifyParentTransformOffsetUpdated(offsetTransform);
                }
            }
        }

        protected void FindAndNotifyAllChildrenParentExists(IReadOnlyList<Child> children)
        {
            var entityId = gameObject.EntityId();
            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index].ChildId;
                TransformChildHierarchyBehaviour hierarchyVisualizer;

                if (TryGetChildHierarchyVisualizer(child, out hierarchyVisualizer))
                {
                    hierarchyVisualizer.NotifyParentExists(entityId, this);
                }
            }
        }

        protected void NotifyAllChildrenParentDisabled()
        {
            foreach (var keyValue in ChildrenVisualizers)
            {
                var listOfChildren = keyValue.Value;
                for (var index = 0; index < listOfChildren.Count; index++)
                {
                    var childHierarchyBehaviour = listOfChildren[index];
                    if (childHierarchyBehaviour != null && childHierarchyBehaviour.isActiveAndEnabled)
                    {
                        childHierarchyBehaviour.NotifyParentDisabled();
                    }
                }
            }
            ChildrenVisualizers.Clear();
        }

        protected bool TryGetChildHierarchyVisualizer(EntityId childId, out TransformChildHierarchyBehaviour transformChildHierarchyBehaviour)
        {
            transformChildHierarchyBehaviour = null;
            var childEntityObject = Universe.Get(childId);
            if (childEntityObject == null)
            {
                return false;
            }

            var childGameObject = childEntityObject.UnderlyingGameObject;
            if (childGameObject == null)
            {
                return false;
            }

            transformChildHierarchyBehaviour = childGameObject.GetComponent<TransformChildHierarchyBehaviour>();
            if (transformChildHierarchyBehaviour == null || !transformChildHierarchyBehaviour.isActiveAndEnabled)
            {
                return false;
            }

            return true;
        }

        protected void AddChildVisualizer(string key, TransformChildHierarchyBehaviour behaviour)
        {
            if (isBeingDisabled)
            {
                return;
            }

            IList<TransformChildHierarchyBehaviour> list;
            if (!ChildrenVisualizers.TryGetValue(key, out list))
            {
                list = new List<TransformChildHierarchyBehaviour>();
                ChildrenVisualizers.Add(key, list);
            }
            list.Add(behaviour);
        }

        protected void RemoveChildVisualizer(string key, TransformChildHierarchyBehaviour behaviour)
        {
            if (isBeingDisabled)
            {
                return;
            }

            IList<TransformChildHierarchyBehaviour> list;
            if (!ChildrenVisualizers.TryGetValue(key, out list))
            {
                return;
            }
            list.Remove(behaviour);
            if (list.Count == 0)
            {
                ChildrenVisualizers.Remove(key);
            }
        }
    }
}