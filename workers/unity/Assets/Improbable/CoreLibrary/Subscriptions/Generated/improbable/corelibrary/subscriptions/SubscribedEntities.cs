// Generated by ProtocGenFabric. DO NOT EDIT!
// source: schema.improbable.corelibrary.subscriptions.SubscribedEntities in improbable/corelibrary/subscriptions/subscribed_entities.proto.

namespace Improbable.Corelibrary.Subscriptions
{
public struct SubscribedEntities : global::System.IEquatable<SubscribedEntities>
{
    public readonly global::Improbable.Util.Collections.IReadOnlyList<Improbable.EntityId> Entities;

    public SubscribedEntities (global::Improbable.Util.Collections.IReadOnlyList<Improbable.EntityId> entities)
    {
        Entities = entities;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is SubscribedEntities))
            return false;
        return Equals((SubscribedEntities) obj);
    }

    public static bool operator ==(SubscribedEntities obj1, SubscribedEntities obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(SubscribedEntities obj1, SubscribedEntities obj2)
    {
        return !obj1.Equals(obj2);
    }

    public bool Equals(SubscribedEntities obj)
    {
        return true
            && global::Improbable.Util.Collections.CollectionUtil.ListsEqual(Entities, obj.Entities);
    }

    public override int GetHashCode()
    {
        int res = 1327;
        res = (res * 977) + (Entities != null ? Entities.GetHashCode() : 0);
        return res;
    }
}

//For internal use only, not to be used in user code.
public sealed class SubscribedEntitiesHelper : global::Improbable.Tools.IProtoConverter<Improbable.Corelibrary.Subscriptions.SubscribedEntities, Schema.Improbable.Corelibrary.Subscriptions.SubscribedEntities>
{
    static readonly SubscribedEntitiesHelper _instance = new SubscribedEntitiesHelper();
    public static SubscribedEntitiesHelper Instance { get { return _instance; } }
    private SubscribedEntitiesHelper() {}

    public Schema.Improbable.Corelibrary.Subscriptions.SubscribedEntities ToProto(Improbable.Corelibrary.Subscriptions.SubscribedEntities data)
    {
        var proto = new Schema.Improbable.Corelibrary.Subscriptions.SubscribedEntities();
        global::Improbable.Tools.ToProto<Improbable.EntityId, long>(data.Entities, proto.Entities, Improbable.EntityIdHelper.Instance);
        return proto;
    }

    //Shallow merge method
    public Improbable.Corelibrary.Subscriptions.SubscribedEntities MergeFromProto(Schema.Improbable.Corelibrary.Subscriptions.SubscribedEntities proto, bool[] statesToClear, Improbable.Corelibrary.Subscriptions.SubscribedEntities data)
    {
        return new Improbable.Corelibrary.Subscriptions.SubscribedEntities(
            (proto.Entities.Count > 0 || statesToClear != null && statesToClear[0]) ? global::Improbable.Tools.FromProto<Improbable.EntityId, long>(proto.Entities, Improbable.EntityIdHelper.Instance) : data.Entities
        );
    }

    public Improbable.Corelibrary.Subscriptions.SubscribedEntities FromProto(Schema.Improbable.Corelibrary.Subscriptions.SubscribedEntities proto)
    {
        return new Improbable.Corelibrary.Subscriptions.SubscribedEntities(
            global::Improbable.Tools.FromProto<Improbable.EntityId, long>(proto.Entities, Improbable.EntityIdHelper.Instance)
        );
    }

    //Shallow merge method
    public void MergeProto(Schema.Improbable.Corelibrary.Subscriptions.SubscribedEntities protoFrom, bool[] statesToClearFrom, Schema.Improbable.Corelibrary.Subscriptions.SubscribedEntities protoTo, bool[] statesToClearTo)
    {
        if ((protoFrom.Entities.Count > 0 || statesToClearFrom != null && statesToClearFrom[0]))
        {
            statesToClearTo[0] = statesToClearFrom[0];
            protoTo.Entities.Clear();
            protoTo.Entities.AddRange(protoFrom.Entities);
        }
    }
}
}
