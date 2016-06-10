//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Option: missing-value detection (*Specified/ShouldSerialize*/Reset*) enabled
    
// Generated from: improbable/color/color_state.proto
// Note: requires additional types generated from: improbable/entity_state.proto
// Note: requires additional types generated from: improbable/math/vector3f.proto
namespace Schema.Improbable.ColorState
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ColorStateData")]
  public partial class ColorStateData : global::ProtoBuf.IExtensible
  {
    public ColorStateData() {}
    

    private Schema.Improbable.Math.Vector3f _value = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"value", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Schema.Improbable.Math.Vector3f Value
    {
      get { return _value; }
      set { _value = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
// Generated from: improbable/player/controls/player_controls_state.proto
// Note: requires additional types generated from: improbable/entity_state.proto
// Note: requires additional types generated from: improbable/math/vector3d.proto
namespace Schema.Improbable.Player.Controls
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PlayerControlsStateData")]
  public partial class PlayerControlsStateData : global::ProtoBuf.IExtensible
  {
    public PlayerControlsStateData() {}
    

    private Schema.Improbable.Math.Vector3d _movementDirection = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"movementDirection", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Schema.Improbable.Math.Vector3d MovementDirection
    {
      get { return _movementDirection; }
      set { _movementDirection = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
// Generated from: improbable/player/local_player_check_state.proto
// Note: requires additional types generated from: improbable/entity_state.proto
namespace Schema.Improbable.Player
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"LocalPlayerCheckStateData")]
  public partial class LocalPlayerCheckStateData : global::ProtoBuf.IExtensible
  {
    public LocalPlayerCheckStateData() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
// Generated from: improbable/player/physical/player_state.proto
// Note: requires additional types generated from: improbable/entity_state.proto
namespace Schema.Improbable.Player.Physical
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PlayerStateData")]
  public partial class PlayerStateData : global::ProtoBuf.IExtensible
  {
    public PlayerStateData() {}
    

    private float? _forceMagnitude;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"forceMagnitude", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float ForceMagnitude
    {
      get { return _forceMagnitude?? default(float); }
      set { _forceMagnitude = value; }
    }
    [global::System.Xml.Serialization.XmlIgnore]
    [global::System.ComponentModel.Browsable(false)]
    public bool ForceMagnitudeSpecified
    {
      get { return _forceMagnitude != null; }
      set { if (value == (_forceMagnitude== null)) _forceMagnitude = value ? ForceMagnitude : (float?)null; }
    }
    private bool ShouldSerializeForceMagnitude() { return ForceMagnitudeSpecified; }
    private void ResetForceMagnitude() { ForceMagnitudeSpecified = false; }
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}