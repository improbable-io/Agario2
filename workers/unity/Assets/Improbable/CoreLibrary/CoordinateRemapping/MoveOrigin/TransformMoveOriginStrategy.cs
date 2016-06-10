using System.Collections.Generic;
using Improbable.Math;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.ComponentFactory;
using UnityEngine;

namespace Improbable.CoreLibrary.CoordinateRemapping.MoveOrigin
{
    /// <summary>
    /// The TransformMoveOriginStrategy is a basic IMoveOriginStrategy which moves every
    /// GameObject which is not parented and not a pool to the new Origin.
    /// </summary>
    public class TransformMoveOriginStrategy : MonoBehaviour, IMoveOriginStrategy
    {
        public virtual void MoveOrigin(Vector3d oldOrigin, Vector3d newOrigin)
        {
            // TODO: (APP-261) When we move to Unity 5.3 change this to
            // UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
            // So that we only iterate over top level scene objects.
            var offset = (oldOrigin - newOrigin).ToUnityVector();
            var allObjects = FindObjectsOfType<GameObject>();
            for (var index = 0; index < allObjects.Length; index++)
            {
                var targetObject = allObjects[index];
                if (ShouldBeMoved(targetObject))
                {
                    targetObject.transform.position += offset;
                }
            }
        }

        public virtual bool ShouldBeMoved(GameObject targetObject)
        {
            return targetObject.activeInHierarchy && !IsParented(targetObject) &&
                   !PooledPrefabContainer.IsPool(targetObject);
        }

        private static bool IsParented(GameObject targetObject)
        {
            var parent = targetObject.transform.parent;
            return parent != null && !PooledPrefabContainer.IsPool(parent.gameObject);
        }
    }
}
