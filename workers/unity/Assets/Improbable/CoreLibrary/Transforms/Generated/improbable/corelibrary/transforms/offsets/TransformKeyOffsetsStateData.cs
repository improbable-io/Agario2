// Generated by ProtocGenFabric. DO NOT EDIT!
// source: schema.improbable.corelibrary.transforms.offsets.TransformKeyOffsetsStateData in improbable/corelibrary/transforms/offsets/transform_key_offsets_state.proto.

namespace Improbable.Corelibrary.Transforms.Offsets
{
public struct TransformKeyOffsetsStateData : global::System.IEquatable<TransformKeyOffsetsStateData>
{
    public readonly global::Improbable.Util.Collections.IReadOnlyDictionary<string, Improbable.Corelibrary.Transforms.Transform> KeyMap;

    public TransformKeyOffsetsStateData (global::Improbable.Util.Collections.IReadOnlyDictionary<string, Improbable.Corelibrary.Transforms.Transform> keyMap)
    {
        KeyMap = keyMap;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TransformKeyOffsetsStateData))
            return false;
        return Equals((TransformKeyOffsetsStateData) obj);
    }

    public static bool operator ==(TransformKeyOffsetsStateData obj1, TransformKeyOffsetsStateData obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(TransformKeyOffsetsStateData obj1, TransformKeyOffsetsStateData obj2)
    {
        return !obj1.Equals(obj2);
    }

    public bool Equals(TransformKeyOffsetsStateData obj)
    {
        return true
            && global::Improbable.Util.Collections.CollectionUtil.DictionariesEqual(KeyMap, obj.KeyMap);
    }

    public override int GetHashCode()
    {
        int res = 1327;
        res = (res * 977) + (KeyMap != null ? KeyMap.GetHashCode() : 0);
        return res;
    }
}

//For internal use only, not to be used in user code.
public sealed class TransformKeyOffsetsStateDataHelper : global::Improbable.Tools.IProtoConverter<Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData, Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData>
{
    static readonly TransformKeyOffsetsStateDataHelper _instance = new TransformKeyOffsetsStateDataHelper();
    public static TransformKeyOffsetsStateDataHelper Instance { get { return _instance; } }
    private TransformKeyOffsetsStateDataHelper() {}

    public Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData ToProto(Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData data)
    {
        var proto = new Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData();
        global::Improbable.Tools.ToProto(data.KeyMap, proto.KeyMap, Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateDataHelper.KeyMapEntryHelper.Instance);
        return proto;
    }

    //Shallow merge method
    public Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData MergeFromProto(Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData proto, bool[] statesToClear, Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData data)
    {
        return new Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData(
            (proto.KeyMap.Count > 0 || statesToClear != null && statesToClear[0]) ? global::Improbable.Tools.FromProto(proto.KeyMap, Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateDataHelper.KeyMapEntryHelper.Instance) : data.KeyMap
        );
    }

    public Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData FromProto(Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData proto)
    {
        return new Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData(
            global::Improbable.Tools.FromProto(proto.KeyMap, Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateDataHelper.KeyMapEntryHelper.Instance)
        );
    }

    //Shallow merge method
    public void MergeProto(Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData protoFrom, bool[] statesToClearFrom, Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData protoTo, bool[] statesToClearTo)
    {
        if ((protoFrom.KeyMap.Count > 0 || statesToClearFrom != null && statesToClearFrom[0]))
        {
            statesToClearTo[0] = statesToClearFrom[0];
            protoTo.KeyMap.Clear();
            protoTo.KeyMap.AddRange(protoFrom.KeyMap);
        }
    }

    //For internal use only, not to be used by user code
    public sealed class KeyMapEntryHelper : global::Improbable.Tools.IProtoKeyValueConverter<string, Improbable.Corelibrary.Transforms.Transform, Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData.KeyMapEntry>
    {
        static readonly KeyMapEntryHelper _instance = new KeyMapEntryHelper();
        public static KeyMapEntryHelper Instance { get { return _instance; } }
        private KeyMapEntryHelper() {}

        public Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData.KeyMapEntry ToProto(System.Collections.Generic.KeyValuePair<string, Improbable.Corelibrary.Transforms.Transform> keyValue)
        {
            var proto = new Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData.KeyMapEntry();
            proto.Key = keyValue.Key;
            proto.Value = Improbable.Corelibrary.Transforms.TransformHelper.Instance.ToProto(keyValue.Value);
            return proto;
        }

        public global::System.Collections.Generic.KeyValuePair<string, Improbable.Corelibrary.Transforms.Transform> FromProto(Schema.Improbable.Corelibrary.Transforms.Offsets.TransformKeyOffsetsStateData.KeyMapEntry proto)
        {
            return new global::System.Collections.Generic.KeyValuePair<string, Improbable.Corelibrary.Transforms.Transform>(proto.Key, Improbable.Corelibrary.Transforms.TransformHelper.Instance.FromProto(proto.Value));
        }
    }
}
}