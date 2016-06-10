using Improbable.Entity.Physical;
using UnityEngine;

//TODO: Change Namespace
namespace Improbable.Corelib.Physical.Visualizers
{
    public interface IRigidbodyVisualizer
    {
        RigidbodyDataReader RigidbodyData { get; }
        Rigidbody Rigidbody { get; }
    }
}
