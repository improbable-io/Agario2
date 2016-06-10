package improbable.natures

import improbable.corelib.natures.{BaseNature, NatureApplication, NatureDescription}
import improbable.corelibrary.rigidbody.RigidbodyNature
import improbable.corelibrary.transforms.TransformNature
import improbable.math.Vector3d
import improbable.papi.entity.behaviour.EntityBehaviourDescriptor
import improbable.util.EntityPrefabs.BOT

object BotNature extends NatureDescription {

  override def dependencies: Set[NatureDescription] = Set(
    BaseNature,
    TransformNature,
    RigidbodyNature,
    ColoredNature
  )

  override def activeBehaviours: Set[EntityBehaviourDescriptor] = Set.empty

  def apply(initialPosition: Vector3d): NatureApplication = {
    application(
      natures = Seq(
        BaseNature(entityPrefab = BOT),
        TransformNature(globalPosition = initialPosition),
        RigidbodyNature(drag = 0.2f),
        ColoredNature(color = java.awt.Color.white))
    )
  }

}
