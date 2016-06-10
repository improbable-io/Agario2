// Generated by ProtocGenFabric. DO NOT EDIT!
// source: schema.improbable.corelibrary.transforms.teleport.TeleportAckStateData in improbable/corelibrary/transforms/teleport/teleport_ack_state.proto.

namespace Improbable.Corelibrary.Transforms.Teleport
{
public struct TeleportAckStateData : global::System.IEquatable<TeleportAckStateData>
{
    public readonly int LastExecutedRequest;

    public TeleportAckStateData (int lastExecutedRequest)
    {
        LastExecutedRequest = lastExecutedRequest;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TeleportAckStateData))
            return false;
        return Equals((TeleportAckStateData) obj);
    }

    public static bool operator ==(TeleportAckStateData obj1, TeleportAckStateData obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(TeleportAckStateData obj1, TeleportAckStateData obj2)
    {
        return !obj1.Equals(obj2);
    }

    public bool Equals(TeleportAckStateData obj)
    {
        return true
            && LastExecutedRequest.Equals(obj.LastExecutedRequest);
    }

    public override int GetHashCode()
    {
        int res = 1327;
        res = (res * 977) + LastExecutedRequest.GetHashCode();
        return res;
    }
}

//For internal use only, not to be used in user code.
public sealed class TeleportAckStateDataHelper : global::Improbable.Tools.IProtoConverter<Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData, Schema.Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData>
{
    static readonly TeleportAckStateDataHelper _instance = new TeleportAckStateDataHelper();
    public static TeleportAckStateDataHelper Instance { get { return _instance; } }
    private TeleportAckStateDataHelper() {}

    public Schema.Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData ToProto(Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData data)
    {
        var proto = new Schema.Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData();
        proto.LastExecutedRequest = data.LastExecutedRequest;
        return proto;
    }

    //Shallow merge method
    public Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData MergeFromProto(Schema.Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData proto, bool[] statesToClear, Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData data)
    {
        return new Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData(
            proto.LastExecutedRequestSpecified ? proto.LastExecutedRequest : data.LastExecutedRequest
        );
    }

    public Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData FromProto(Schema.Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData proto)
    {
        return new Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData(
            proto.LastExecutedRequest
        );
    }

    //Shallow merge method
    public void MergeProto(Schema.Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData protoFrom, bool[] statesToClearFrom, Schema.Improbable.Corelibrary.Transforms.Teleport.TeleportAckStateData protoTo, bool[] statesToClearTo)
    {
        if (protoFrom.LastExecutedRequestSpecified)
        {
            protoTo.LastExecutedRequest = protoFrom.LastExecutedRequest;
            protoTo.LastExecutedRequestSpecified = protoFrom.LastExecutedRequestSpecified;
        }
    }
}
}
