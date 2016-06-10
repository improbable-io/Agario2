// This class is copied from the class generated from the .proto

// The name of this file cannot be Transform.cs or else
// Unity throws a warning.
#region Generated Code
namespace Improbable.Corelibrary.Transforms
{
    public struct Transform : global::System.IEquatable<Transform>
    {
        public readonly Improbable.Math.Vector3d Position;
        public readonly Improbable.Corelib.Math.Quaternion Rotation;

        public Transform (Improbable.Math.Vector3d position,
            Improbable.Corelib.Math.Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Transform))
                return false;
            return Equals((Transform) obj);
        }

        public bool Equals(Transform obj)
        {
            return true
                && Position.Equals(obj.Position)
                && Rotation.Equals(obj.Rotation);
        }

        public static bool operator ==(Transform obj1, Transform obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(Transform obj1, Transform obj2)
        {
            return !obj1.Equals(obj2);
        }

        public override int GetHashCode()
        {
            int res = 1327;
            res = (res * 977) + Position.GetHashCode();
            res = (res * 977) + Rotation.GetHashCode();
            return res;
        }
    }

    //For internal use only, not to be used in user code.
    public sealed class TransformHelper : global::Improbable.Tools.IProtoConverter<Improbable.Corelibrary.Transforms.Transform, Schema.Improbable.Corelibrary.Transforms.Transform>
    {
        static readonly TransformHelper _instance = new TransformHelper();
        public static TransformHelper Instance { get { return _instance; } }
        private TransformHelper() {}

        public Schema.Improbable.Corelibrary.Transforms.Transform ToProto(Improbable.Corelibrary.Transforms.Transform data)
        {
            var proto = new Schema.Improbable.Corelibrary.Transforms.Transform();
            proto.Position = Improbable.Math.Vector3dHelper.Instance.ToProto(data.Position);
            proto.Rotation = Improbable.Corelib.Math.QuaternionHelper.Instance.ToProto(data.Rotation);
            return proto;
        }

        //Shallow merge method
        public Improbable.Corelibrary.Transforms.Transform MergeFromProto(Schema.Improbable.Corelibrary.Transforms.Transform proto, bool[] statesToClear, Improbable.Corelibrary.Transforms.Transform data)
        {
            return new Improbable.Corelibrary.Transforms.Transform(
                proto.Position != null ? Improbable.Math.Vector3dHelper.Instance.FromProto(proto.Position) : data.Position,
                proto.Rotation != null ? Improbable.Corelib.Math.QuaternionHelper.Instance.FromProto(proto.Rotation) : data.Rotation
            );
        }

        public Improbable.Corelibrary.Transforms.Transform FromProto(Schema.Improbable.Corelibrary.Transforms.Transform proto)
        {
            return new Improbable.Corelibrary.Transforms.Transform(
                Improbable.Math.Vector3dHelper.Instance.FromProto(proto.Position),
                Improbable.Corelib.Math.QuaternionHelper.Instance.FromProto(proto.Rotation)
            );
        }

        //Shallow merge method
        public void MergeProto(Schema.Improbable.Corelibrary.Transforms.Transform protoFrom, bool[] statesToClearFrom, Schema.Improbable.Corelibrary.Transforms.Transform protoTo, bool[] statesToClearTo)
        {
            if (protoFrom.Position != null)
            {
                protoTo.Position = protoFrom.Position;
            }
            if (protoFrom.Rotation != null)
            {
                protoTo.Rotation = protoFrom.Rotation;
            }
        }
    }
}
#endregion