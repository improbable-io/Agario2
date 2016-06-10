using Improbable.ColorState;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers
{
    public class ColorVisualizer : MonoBehaviour
    {
        [Require] protected ColorStateReader Color;

        public Renderer[] Renderers;

        protected void OnEnable()
        {
            Color.ValueUpdated += HandleValueUpdated;
        }

        protected void OnDisable()
        {
            Color.ValueUpdated -= HandleValueUpdated;
        }

        private void HandleValueUpdated(Vector3f color)
        {
            if (Renderers == null)
            {
                return;
            }

            foreach (var renderer in Renderers)
            {
                renderer.material.color = new Color(color.X, color.Y, color.Z);
            }
        }
    }
}