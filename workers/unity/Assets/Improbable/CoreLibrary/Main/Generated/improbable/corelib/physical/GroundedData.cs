// Generated by ProtocGenFabric. DO NOT EDIT!
// source: schema.improbable.corelib.physical.GroundedData in improbable/corelib/physical/grounded.proto.

namespace Improbable.Corelib.Physical
{
public struct GroundedData : global::System.IEquatable<GroundedData>
{
    public readonly Improbable.EntityId GroundEntityId;
    public readonly bool IsGrounded;
    public readonly float MaximumInclineDegrees;

    public GroundedData (Improbable.EntityId groundEntityId,
        bool isGrounded,
        float maximumInclineDegrees)
    {
        GroundEntityId = groundEntityId;
        IsGrounded = isGrounded;
        MaximumInclineDegrees = maximumInclineDegrees;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is GroundedData))
            return false;
        return Equals((GroundedData) obj);
    }

    public static bool operator ==(GroundedData obj1, GroundedData obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(GroundedData obj1, GroundedData obj2)
    {
        return !obj1.Equals(obj2);
    }

    public bool Equals(GroundedData obj)
    {
        return true
            && GroundEntityId.Equals(obj.GroundEntityId)
            && IsGrounded.Equals(obj.IsGrounded)
            && MaximumInclineDegrees.Equals(obj.MaximumInclineDegrees);
    }

    public override int GetHashCode()
    {
        int res = 1327;
        res = (res * 977) + GroundEntityId.GetHashCode();
        res = (res * 977) + IsGrounded.GetHashCode();
        res = (res * 977) + MaximumInclineDegrees.GetHashCode();
        return res;
    }
}

//For internal use only, not to be used in user code.
public sealed class GroundedDataHelper : global::Improbable.Tools.IProtoConverter<Improbable.Corelib.Physical.GroundedData, Schema.Improbable.Corelib.Physical.GroundedData>
{
    static readonly GroundedDataHelper _instance = new GroundedDataHelper();
    public static GroundedDataHelper Instance { get { return _instance; } }
    private GroundedDataHelper() {}

    public Schema.Improbable.Corelib.Physical.GroundedData ToProto(Improbable.Corelib.Physical.GroundedData data)
    {
        var proto = new Schema.Improbable.Corelib.Physical.GroundedData();
        proto.GroundEntityId = Improbable.EntityIdHelper.Instance.ToProto(data.GroundEntityId);
        proto.IsGrounded = data.IsGrounded;
        proto.MaximumInclineDegrees = data.MaximumInclineDegrees;
        return proto;
    }

    //Shallow merge method
    public Improbable.Corelib.Physical.GroundedData MergeFromProto(Schema.Improbable.Corelib.Physical.GroundedData proto, bool[] statesToClear, Improbable.Corelib.Physical.GroundedData data)
    {
        return new Improbable.Corelib.Physical.GroundedData(
            proto.GroundEntityIdSpecified ? Improbable.EntityIdHelper.Instance.FromProto(proto.GroundEntityId) : data.GroundEntityId,
            proto.IsGroundedSpecified ? proto.IsGrounded : data.IsGrounded,
            proto.MaximumInclineDegreesSpecified ? proto.MaximumInclineDegrees : data.MaximumInclineDegrees
        );
    }

    public Improbable.Corelib.Physical.GroundedData FromProto(Schema.Improbable.Corelib.Physical.GroundedData proto)
    {
        return new Improbable.Corelib.Physical.GroundedData(
            Improbable.EntityIdHelper.Instance.FromProto(proto.GroundEntityId),
            proto.IsGrounded,
            proto.MaximumInclineDegrees
        );
    }

    //Shallow merge method
    public void MergeProto(Schema.Improbable.Corelib.Physical.GroundedData protoFrom, bool[] statesToClearFrom, Schema.Improbable.Corelib.Physical.GroundedData protoTo, bool[] statesToClearTo)
    {
        if (protoFrom.GroundEntityIdSpecified)
        {
            protoTo.GroundEntityId = protoFrom.GroundEntityId;
            protoTo.GroundEntityIdSpecified = protoFrom.GroundEntityIdSpecified;
        }
        if (protoFrom.IsGroundedSpecified)
        {
            protoTo.IsGrounded = protoFrom.IsGrounded;
            protoTo.IsGroundedSpecified = protoFrom.IsGroundedSpecified;
        }
        if (protoFrom.MaximumInclineDegreesSpecified)
        {
            protoTo.MaximumInclineDegrees = protoFrom.MaximumInclineDegrees;
            protoTo.MaximumInclineDegreesSpecified = protoFrom.MaximumInclineDegreesSpecified;
        }
    }
}
}