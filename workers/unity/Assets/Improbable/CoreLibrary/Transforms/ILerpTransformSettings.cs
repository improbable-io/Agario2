namespace Improbable.CoreLibrary.Transforms
{
    public interface ILerpTransformSettings
    {
        float GetInterpolationDelayInSeconds();
        float GetMinAngleToInterpolateBetween();
        float GetMinDistanceToInterpolateBetween();
        float GetMaxSecondsToInterpolateAfterLastUpdate();
    }
}