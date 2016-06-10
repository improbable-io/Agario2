
using UnityEngine;

namespace Improbable.CoreLibrary.Transforms
{
    public interface IUpdateTransformSettings
    {
        bool GetUseCenterOfMassPivot();
        Vector3 GetPivotOffset();
        float GetNetworkUpdatePeriodThreshold();
        float GetPositionNetworkUpdateSquareDistanceThreshold();
        float GetRotationNetworkUpdateAngleThreshold();
    }
}