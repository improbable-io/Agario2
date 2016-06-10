// Generated by SpatialOS codegen. DO NOT EDIT!
// source: schema.improbable.corelib.entity.PrefabData in improbable/corelib/entity/prefab.proto.

using System;
using Improbable.Core.Serialization;
using Improbable.Entity.State;

namespace Improbable.Corelib.Entity
{
[ReaderInterface]
[CanonicalName("improbable.corelib.entity.Prefab", 190218)]
public interface PrefabReader : IEntityStateReader
{
    string Name { get; }

    event System.Action<string> NameUpdated;
}

public interface IPrefabUpdater : IEntityStateUpdater
{
    void FinishAndSend();
    IPrefabUpdater Name(string newValue);
}

[WriterInterface]
[CanonicalName("improbable.corelib.entity.Prefab", 190218)]
public interface PrefabWriter : PrefabReader, IUpdateable<IPrefabUpdater> { }

public class Prefab : global::Improbable.Entity.State.StateBase<Improbable.Corelib.Entity.PrefabData, Schema.Improbable.Corelib.Entity.PrefabData>, PrefabWriter, IPrefabUpdater
{
    public Prefab(global::Improbable.EntityId entityId, Improbable.Corelib.Entity.PrefabData data, IStateSender sender)
        : base(entityId, data, sender, Improbable.Corelib.Entity.PrefabDataHelper.Instance) { }
    private static log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(Prefab));
    private static bool ShouldLogFinishAndSendNoUpdate = true;
    private static bool ShouldLogUpdateNoFinishAndSend = true;

    protected override void LogFinishAndSendWithNoUpdate() {
        if (ShouldLogFinishAndSendNoUpdate)
        {
            ShouldLogFinishAndSendNoUpdate = false;
            LOGGER.ErrorFormat("Finish and send was called with no update in flight for entity {0}. " +
                               "This is probably due to having more StateUpdates in flight, which is an error. (Logged only once.)", EntityId);
        }
    }

    public string Name { get { return Data.Name; } }

    private readonly global::System.Collections.Generic.List<System.Action<string>> updatedCallbacksName =
        new global::System.Collections.Generic.List<System.Action<string>>();
    public event System.Action<string> NameUpdated
    {
        add
        {
            updatedCallbacksName.Add(value);
            value(Data.Name);
        }
        remove { updatedCallbacksName.Remove(value); }
    }

    override protected void UnsubscribeEventHandlersInternal(object visualizer)
    {
        UnsubscribeEventHandler(visualizer, updatedCallbacksName);
    }

    public IPrefabUpdater Update
    {
        get
        {
            if (Updating)
            {
                if (ShouldLogUpdateNoFinishAndSend)
                {
                    ShouldLogUpdateNoFinishAndSend = false;
                    LOGGER.ErrorFormat("Multiple state updates of entity {0} are in flight, which has undefined semantics. " +
                        "Each call to Update has to be followed by a FinishAndSend() before another call is made on the same state. (Logged only once.)", EntityId);
                }
            }
            else
            {
                Updating = true;
                Updater = new PrefabUpdate(EntityId, new bool[0], new Schema.Improbable.Corelib.Entity.PrefabData());
            }
            return this;
        }
    }

    IPrefabUpdater IPrefabUpdater.Name(string newValue)
    {
        if (Updater.Proto.NameSpecified || !string.Equals(Name, newValue))
        {
            Updater.Proto.Name = newValue;
        }
        return this;
    }

    override protected bool TriggerUpdatedEvents(Schema.Improbable.Corelib.Entity.PrefabData update, bool[] statesToClear)
    {
        bool anythingUpdated = false;
        bool updatedName = update.NameSpecified;
        anythingUpdated |= updatedName;
        if (updatedName) TriggerCallbacks(updatedCallbacksName, Data.Name);

        if (anythingUpdated) TriggerPropertyUpdated();
        return anythingUpdated;
    }

    override protected bool TriggerAllStateEvents(Schema.Improbable.Corelib.Entity.PrefabData stateUpdate)
    {
        bool anythingUpdated = false;
        return anythingUpdated;
    }

    override protected bool TriggerAllStateEvents()
    {
        bool anythingUpdated = false;
        return anythingUpdated;
    }
}

public class PrefabUpdate : global::Improbable.Entity.State.StateUpdate<Improbable.Corelib.Entity.PrefabData, Schema.Improbable.Corelib.Entity.PrefabData>
{
    public const uint COMPONENT_ID = 190218;
    public PrefabUpdate(global::Improbable.EntityId entityId, bool[] statesToClear, Schema.Improbable.Corelib.Entity.PrefabData proto)
        : base(entityId, statesToClear, Improbable.Corelib.Entity.PrefabDataHelper.Instance, proto, COMPONENT_ID) { }

    public override IReadWriteEntityState CreateState(global::Improbable.EntityId entityId, IStateSender stateSender)
    {
        return new Prefab(entityId, GetData(), stateSender);
    }

    public static PrefabUpdate ExtractFrom(global::Improbable.Protocol.ComponentUpdate proto)
    {
        var protoState = ProtoBuf.Extensible.GetValue<Schema.Improbable.Corelib.Entity.PrefabData>(proto.ComponentData, (int) COMPONENT_ID);
        return new PrefabUpdate(global::Improbable.EntityIdHelper.Instance.FromProto(proto.EntityId), null, protoState);
    }

    override protected int SeqToId(int seqId) { return seqToId[seqId]; }
    private static int[] seqToId = {};
}
}