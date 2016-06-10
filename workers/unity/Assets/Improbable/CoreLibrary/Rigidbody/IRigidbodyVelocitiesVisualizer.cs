using Improbable.Entity.Physical;
using UnityEngine;

//TODO: Change Namespace
namespace Improbable.Corelib.Physical.Visualizers
{
    public interface IRigidbodyVelocitiesVisualizer
    {
        RigidbodyEngineDataReader RigidbodyEngineData { get; }
        Rigidbody Rigidbody { get; }
    }
}