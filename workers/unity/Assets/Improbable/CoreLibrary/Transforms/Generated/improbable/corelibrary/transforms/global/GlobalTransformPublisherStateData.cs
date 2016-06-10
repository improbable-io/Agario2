// Generated by ProtocGenFabric. DO NOT EDIT!
// source: schema.improbable.corelibrary.transforms.global.GlobalTransformPublisherStateData in improbable/corelibrary/transforms/global/global_transform_publisher_state.proto.

namespace Improbable.Corelibrary.Transforms.Global
{
public struct GlobalTransformPublisherStateData : global::System.IEquatable<GlobalTransformPublisherStateData>
{
    public readonly Improbable.Corelibrary.Subscriptions.SubscriberData SubscriberData;

    public GlobalTransformPublisherStateData (Improbable.Corelibrary.Subscriptions.SubscriberData subscriberData)
    {
        SubscriberData = subscriberData;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is GlobalTransformPublisherStateData))
            return false;
        return Equals((GlobalTransformPublisherStateData) obj);
    }

    public static bool operator ==(GlobalTransformPublisherStateData obj1, GlobalTransformPublisherStateData obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(GlobalTransformPublisherStateData obj1, GlobalTransformPublisherStateData obj2)
    {
        return !obj1.Equals(obj2);
    }

    public bool Equals(GlobalTransformPublisherStateData obj)
    {
        return true
            && SubscriberData.Equals(obj.SubscriberData);
    }

    public override int GetHashCode()
    {
        int res = 1327;
        res = (res * 977) + SubscriberData.GetHashCode();
        return res;
    }
}

//For internal use only, not to be used in user code.
public sealed class GlobalTransformPublisherStateDataHelper : global::Improbable.Tools.IProtoConverter<Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData, Schema.Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData>
{
    static readonly GlobalTransformPublisherStateDataHelper _instance = new GlobalTransformPublisherStateDataHelper();
    public static GlobalTransformPublisherStateDataHelper Instance { get { return _instance; } }
    private GlobalTransformPublisherStateDataHelper() {}

    public Schema.Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData ToProto(Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData data)
    {
        var proto = new Schema.Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData();
        proto.SubscriberData = Improbable.Corelibrary.Subscriptions.SubscriberDataHelper.Instance.ToProto(data.SubscriberData);
        return proto;
    }

    //Shallow merge method
    public Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData MergeFromProto(Schema.Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData proto, bool[] statesToClear, Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData data)
    {
        return new Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData(
            proto.SubscriberData != null ? Improbable.Corelibrary.Subscriptions.SubscriberDataHelper.Instance.FromProto(proto.SubscriberData) : data.SubscriberData
        );
    }

    public Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData FromProto(Schema.Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData proto)
    {
        return new Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData(
            Improbable.Corelibrary.Subscriptions.SubscriberDataHelper.Instance.FromProto(proto.SubscriberData)
        );
    }

    //Shallow merge method
    public void MergeProto(Schema.Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData protoFrom, bool[] statesToClearFrom, Schema.Improbable.Corelibrary.Transforms.Global.GlobalTransformPublisherStateData protoTo, bool[] statesToClearTo)
    {
        if (protoFrom.SubscriberData != null)
        {
            protoTo.SubscriberData = protoFrom.SubscriberData;
        }
    }
}
}