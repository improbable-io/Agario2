using Improbable.Math;

namespace Improbable.CoreLibrary.CoordinateRemapping.MoveOrigin
{
    public interface IMoveOriginStrategy
    {
        void MoveOrigin(Vector3d oldOrigin, Vector3d newOrigin);
    }
}