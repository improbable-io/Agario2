using System;
using System.Collections.Generic;
using System.Linq;
using Improbable.Core.Entity;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.ComponentFactory;
using Improbable.Unity.Entity;
using UnityEngine;

namespace Improbable.CoreLibrary.CoordinateRemapping.DetermineOrigin
{
    /// <summary>
    /// The EntityBounds struct is used in EntityBounds DetermineOriginStrategies. It represents the
    /// minimum and maximum value for each axis.
    /// 
    /// The new origin location is determined by the mid point of the range of values for each axis.
    /// </summary>
    public struct EntityBounds : IEquatable<EntityBounds>
    {
        public readonly Vector3 MinVector;
        public readonly Vector3 MaxVector;

        public EntityBounds(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            MinVector = new Vector3(minX, minY, minZ);
            MaxVector = new Vector3(maxX, maxY, maxZ);
        }

        public EntityBounds(Vector3 minVector, Vector3 maxVector)
        {
            MinVector = minVector;
            MaxVector = maxVector;
        }

        public Vector3 MidVector()
        {
            return (MinVector - MaxVector) / 2f;
        }

        public float GetMaxAbsoluteAxisValue()
        {
            var max = Mathf.Abs(MinVector.x);
            max = Mathf.Max(max, Mathf.Abs(MinVector.y));
            max = Mathf.Max(max, Mathf.Abs(MinVector.z));
            max = Mathf.Max(max, Mathf.Abs(MaxVector.x));
            max = Mathf.Max(max, Mathf.Abs(MaxVector.y));
            max = Mathf.Max(max, Mathf.Abs(MaxVector.z));
            return max;
        }

        public EntityBounds OffsetBy(Vector3 offset)
        {
            return new EntityBounds(MinVector - offset, MaxVector - offset);
        }

        public static EntityBounds CalculateBoundsOfAllEntities(IUniverse Universe, Func<IEntityObject, bool> ShouldConsiderObject)
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var minZ = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            var maxZ = float.MinValue;

            var isEmpty = true;

            Universe.IterateOverAllEntityObjects((entityId, entityObject) =>
            {
                if (entityObject == null)
                {
                    return;
                }

                if (!ShouldConsiderObject(entityObject))
                {
                    return;
                }

                var gameObject = entityObject.UnderlyingGameObject;
                if (gameObject == null)
                {
                    return;
                }

                var position = gameObject
                    .transform
                    .position;

                if (!position.IsFinite())
                {
                    return;
                }

                if (position.Equals(PooledPrefabFactory.InstantiationPoint))
                {
                    return;
                }

                isEmpty = false;

                if (position.x < minX)
                {
                    minX = position.x;
                }
                if (position.x > maxX)
                {
                    maxX = position.x;
                }

                if (position.y < minY)
                {
                    minY = position.y;
                }
                if (position.y > maxY)
                {
                    maxY = position.y;
                }

                if (position.z < minZ)
                {
                    minZ = position.z;
                }
                if (position.z > maxZ)
                {
                    maxZ = position.z;
                }
            });

            return isEmpty ? new EntityBounds() : new EntityBounds(minX, minY, minZ, maxX, maxY, maxZ);
        }

        public bool Equals(EntityBounds other)
        {
            return MinVector.Equals(other.MinVector) && MaxVector.Equals(other.MaxVector);
        }

        public override int GetHashCode()
        {
            var res = 1327;
            res = (res * 977) + MinVector.GetHashCode();
            res = (res * 977) + MaxVector.GetHashCode();
            return res;
        }
    }
}