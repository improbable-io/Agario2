using Improbable.Math;

// This class is copied from the class generated from the .proto

namespace Improbable.Corelib.Math
{
    public struct Quaternion : global::System.IEquatable<Quaternion>
    {
        public static Quaternion Identity = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);

        public static Vector3d operator *(Quaternion r, Vector3d v)
        {
            var u = new Vector3d(r.X, r.Y, r.Z);
            var s = r.W;

            return u * (2.0 * u.Dot(v)) +
                   v * (s * s - u.Dot(u)) +
                   u.Cross(v) * (2.0 * s);
        }

        #region Generated Code
        public readonly float W;
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public Quaternion (float w,
            float x,
            float y,
            float z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Quaternion))
                return false;
            return Equals((Quaternion) obj);
        }

        public bool Equals(Quaternion obj)
        {
            return true
                && W.Equals(obj.W)
                && X.Equals(obj.X)
                && Y.Equals(obj.Y)
                && Z.Equals(obj.Z);
        }

        public static bool operator ==(Quaternion obj1, Quaternion obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(Quaternion obj1, Quaternion obj2)
        {
            return !obj1.Equals(obj2);
        }

        public override int GetHashCode()
        {
            int res = 1327;
            res = (res * 977) + W.GetHashCode();
            res = (res * 977) + X.GetHashCode();
            res = (res * 977) + Y.GetHashCode();
            res = (res * 977) + Z.GetHashCode();
            return res;
        }
        #endregion
    }

    #region Generated Code
    //For internal use only, not to be used in user code.
    public sealed class QuaternionHelper : global::Improbable.Tools.IProtoConverter<Improbable.Corelib.Math.Quaternion, Schema.Improbable.Corelib.Math.Quaternion>
    {
        static readonly QuaternionHelper _instance = new QuaternionHelper();
        public static QuaternionHelper Instance { get { return _instance; } }
        private QuaternionHelper() {}

        public Schema.Improbable.Corelib.Math.Quaternion ToProto(Improbable.Corelib.Math.Quaternion data)
        {
            var proto = new Schema.Improbable.Corelib.Math.Quaternion();
            proto.W = data.W;
            proto.X = data.X;
            proto.Y = data.Y;
            proto.Z = data.Z;
            return proto;
        }

        //Shallow merge method
        public Improbable.Corelib.Math.Quaternion MergeFromProto(Schema.Improbable.Corelib.Math.Quaternion proto, bool[] statesToClear, Improbable.Corelib.Math.Quaternion data)
        {
            return new Improbable.Corelib.Math.Quaternion(
                proto.WSpecified ? proto.W : data.W,
                proto.XSpecified ? proto.X : data.X,
                proto.YSpecified ? proto.Y : data.Y,
                proto.ZSpecified ? proto.Z : data.Z
            );
        }

        public Improbable.Corelib.Math.Quaternion FromProto(Schema.Improbable.Corelib.Math.Quaternion proto)
        {
            return new Improbable.Corelib.Math.Quaternion(
                proto.W,
                proto.X,
                proto.Y,
                proto.Z
            );
        }

        //Shallow merge method
        public void MergeProto(Schema.Improbable.Corelib.Math.Quaternion protoFrom, bool[] statesToClearFrom, Schema.Improbable.Corelib.Math.Quaternion protoTo, bool[] statesToClearTo)
        {
            if (protoFrom.WSpecified)
            {
                protoTo.W = protoFrom.W;
                protoTo.WSpecified = protoFrom.WSpecified;
            }
            if (protoFrom.XSpecified)
            {
                protoTo.X = protoFrom.X;
                protoTo.XSpecified = protoFrom.XSpecified;
            }
            if (protoFrom.YSpecified)
            {
                protoTo.Y = protoFrom.Y;
                protoTo.YSpecified = protoFrom.YSpecified;
            }
            if (protoFrom.ZSpecified)
            {
                protoTo.Z = protoFrom.Z;
                protoTo.ZSpecified = protoFrom.ZSpecified;
            }
        }
    }
    #endregion
}