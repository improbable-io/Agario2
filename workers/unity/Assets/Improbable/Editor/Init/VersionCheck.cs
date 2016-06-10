using UnityEditor;
using UnityEngine;

namespace Improbable.Unity.EditorTools.Init
{
    [InitializeOnLoad]
    public class VersionCheck
    {
        static VersionCheck()
        {
#if !(UNITY_5_3 || UNITY_5_2)
            Debug.LogWarning("SpatialOS SDK: You are using an unsupported version of Unity. Currently supported versions are 5.2 and 5.3.");
#endif
        }
    }
}
