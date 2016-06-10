// Generated by ProtocGenFabric. DO NOT EDIT!
// source: schema.improbable.corelibrary.transforms.TransformStateData in improbable/corelibrary/transforms/transform_state.proto.

namespace Improbable.Corelibrary.Transforms
{
public struct TransformStateData : global::System.IEquatable<TransformStateData>
{
    public readonly Improbable.Math.Vector3d LocalPosition;
    public readonly Improbable.Corelib.Math.Quaternion LocalRotation;
    public readonly Improbable.Corelibrary.Transforms.Parent? Parent;
    public readonly Improbable.Math.Vector3d Pivot;
    public readonly float Timestamp;

    public TransformStateData (Improbable.Math.Vector3d localPosition,
        Improbable.Corelib.Math.Quaternion localRotation,
        Improbable.Corelibrary.Transforms.Parent? parent,
        Improbable.Math.Vector3d pivot,
        float timestamp)
    {
        LocalPosition = localPosition;
        LocalRotation = localRotation;
        Parent = parent;
        Pivot = pivot;
        Timestamp = timestamp;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TransformStateData))
            return false;
        return Equals((TransformStateData) obj);
    }

    public static bool operator ==(TransformStateData obj1, TransformStateData obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(TransformStateData obj1, TransformStateData obj2)
    {
        return !obj1.Equals(obj2);
    }

    public bool Equals(TransformStateData obj)
    {
        return true
            && LocalPosition.Equals(obj.LocalPosition)
            && LocalRotation.Equals(obj.LocalRotation)
            && global::Improbable.Util.Collections.CollectionUtil.OptionsEqual(Parent, obj.Parent)
            && Pivot.Equals(obj.Pivot)
            && Timestamp.Equals(obj.Timestamp);
    }

    public override int GetHashCode()
    {
        int res = 1327;
        res = (res * 977) + LocalPosition.GetHashCode();
        res = (res * 977) + LocalRotation.GetHashCode();
        res = (res * 977) + (Parent != null ? Parent.GetHashCode() : 0);
        res = (res * 977) + Pivot.GetHashCode();
        res = (res * 977) + Timestamp.GetHashCode();
        return res;
    }
}

//For internal use only, not to be used in user code.
public sealed class TransformStateDataHelper : global::Improbable.Tools.IProtoConverter<Improbable.Corelibrary.Transforms.TransformStateData, Schema.Improbable.Corelibrary.Transforms.TransformStateData>
{
    static readonly TransformStateDataHelper _instance = new TransformStateDataHelper();
    public static TransformStateDataHelper Instance { get { return _instance; } }
    private TransformStateDataHelper() {}

    public Schema.Improbable.Corelibrary.Transforms.TransformStateData ToProto(Improbable.Corelibrary.Transforms.TransformStateData data)
    {
        var proto = new Schema.Improbable.Corelibrary.Transforms.TransformStateData();
        proto.LocalPosition = Improbable.Math.Vector3dHelper.Instance.ToProto(data.LocalPosition);
        proto.LocalRotation = Improbable.Corelib.Math.QuaternionHelper.Instance.ToProto(data.LocalRotation);
        if (data.Parent != null) proto.Parent = Improbable.Corelibrary.Transforms.ParentHelper.Instance.ToProto(data.Parent.Value);
        proto.Pivot = Improbable.Math.Vector3dHelper.Instance.ToProto(data.Pivot);
        proto.Timestamp = data.Timestamp;
        return proto;
    }

    //Shallow merge method
    public Improbable.Corelibrary.Transforms.TransformStateData MergeFromProto(Schema.Improbable.Corelibrary.Transforms.TransformStateData proto, bool[] statesToClear, Improbable.Corelibrary.Transforms.TransformStateData data)
    {
        return new Improbable.Corelibrary.Transforms.TransformStateData(
            proto.LocalPosition != null ? Improbable.Math.Vector3dHelper.Instance.FromProto(proto.LocalPosition) : data.LocalPosition,
            proto.LocalRotation != null ? Improbable.Corelib.Math.QuaternionHelper.Instance.FromProto(proto.LocalRotation) : data.LocalRotation,
            (proto.Parent != null || statesToClear != null && statesToClear[0]) ? (proto.Parent == null ? (Improbable.Corelibrary.Transforms.Parent?) null : Improbable.Corelibrary.Transforms.ParentHelper.Instance.FromProto(proto.Parent)) : data.Parent,
            proto.Pivot != null ? Improbable.Math.Vector3dHelper.Instance.FromProto(proto.Pivot) : data.Pivot,
            proto.TimestampSpecified ? proto.Timestamp : data.Timestamp
        );
    }

    public Improbable.Corelibrary.Transforms.TransformStateData FromProto(Schema.Improbable.Corelibrary.Transforms.TransformStateData proto)
    {
        return new Improbable.Corelibrary.Transforms.TransformStateData(
            Improbable.Math.Vector3dHelper.Instance.FromProto(proto.LocalPosition),
            Improbable.Corelib.Math.QuaternionHelper.Instance.FromProto(proto.LocalRotation),
            proto.Parent == null ? (Improbable.Corelibrary.Transforms.Parent?) null : Improbable.Corelibrary.Transforms.ParentHelper.Instance.FromProto(proto.Parent),
            Improbable.Math.Vector3dHelper.Instance.FromProto(proto.Pivot),
            proto.Timestamp
        );
    }

    //Shallow merge method
    public void MergeProto(Schema.Improbable.Corelibrary.Transforms.TransformStateData protoFrom, bool[] statesToClearFrom, Schema.Improbable.Corelibrary.Transforms.TransformStateData protoTo, bool[] statesToClearTo)
    {
        if (protoFrom.LocalPosition != null)
        {
            protoTo.LocalPosition = protoFrom.LocalPosition;
        }
        if (protoFrom.LocalRotation != null)
        {
            protoTo.LocalRotation = protoFrom.LocalRotation;
        }
        if ((protoFrom.Parent != null || statesToClearFrom != null && statesToClearFrom[0]))
        {
            statesToClearTo[0] = statesToClearFrom[0];
            protoTo.Parent = protoFrom.Parent;
        }
        if (protoFrom.Pivot != null)
        {
            protoTo.Pivot = protoFrom.Pivot;
        }
        if (protoFrom.TimestampSpecified)
        {
            protoTo.Timestamp = protoFrom.Timestamp;
            protoTo.TimestampSpecified = protoFrom.TimestampSpecified;
        }
    }
}
}
