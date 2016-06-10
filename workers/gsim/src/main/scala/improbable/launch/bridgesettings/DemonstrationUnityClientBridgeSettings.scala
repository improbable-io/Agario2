package improbable.launch.bridgesettings

import improbable.fapi.bridge.{BridgeSettings, BridgeSettingsResolver, ConstantEngineLoadPolicy, PerEntityOrderedStateUpdateQos}
import improbable.fapi.network.RakNetLinkSettings
import improbable.unity.fabric.engine.EnginePlatform._
import improbable.unity.fabric.satisfiers.{AggregateSatisfiers, SatisfySingleConstraint, SatisfySpecificEngine}
import improbable.unity.fabric.{AuthoritativeEntityOnly, VisualEngineConstraint}

object DemonstrationUnityClientBridgeSettings extends BridgeSettingsResolver {

  private def clientEngineBridgeSettings = BridgeSettings(
    DemonstrationClientAssetContextDiscriminator(),
    RakNetLinkSettings(),
    UNITY_CLIENT_ENGINE,
    AggregateSatisfiers(
      SatisfySpecificEngine,
      SatisfySingleConstraint(VisualEngineConstraint)
    ),
    AuthoritativeEntityOnly(),
    ConstantEngineLoadPolicy(0.5),
    PerEntityOrderedStateUpdateQos
  )

  override def engineTypeToBridgeSettings(engineType: String, metadata: String): Option[BridgeSettings] = {
    engineType match {
      case UNITY_CLIENT_ENGINE =>
        Some(clientEngineBridgeSettings)
      case _ =>
        None
    }
  }

}
