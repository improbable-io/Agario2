using UnityEngine;
using System.Collections;

namespace Improbable.CoreLibrary.Transforms.Offsets
{
    public interface ITransformOffsetsUpdateSettings
    {
        float GetRotationNetworkUpdateAngleThreshold();
        float GetPositionNetworkUpdateSquareDistanceThreshold();
        float GetNetworkUpdatePeriodThresholdInSeconds();
        bool GetOnlyUpdateWhenUsed();
    }
}