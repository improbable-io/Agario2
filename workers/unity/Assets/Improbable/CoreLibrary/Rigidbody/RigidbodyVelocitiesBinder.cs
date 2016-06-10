using System;
using Improbable.Unity.Common.Core.Math;
using UnityEngine;

//TODO: Change Namespace
namespace Improbable.Corelib.Physical.Visualizers
{
    public class RigidbodyVelocitiesBinder
    {
        private readonly IRigidbodyVelocitiesVisualizer rigidbodyVisualizer;
        private bool initialVelocitiesJustApplied;

        public bool InitialVelocitiesApplied { get; private set; }

        public RigidbodyVelocitiesBinder(IRigidbodyVelocitiesVisualizer rigidbodyVisualizer)
        {
            this.rigidbodyVisualizer = rigidbodyVisualizer;
        }

        public void Initialize()
        {
            rigidbodyVisualizer.Rigidbody.Sleep();
            rigidbodyVisualizer.RigidbodyEngineData.AuthorityChanged += AuthorityChanged;
        }

        public void StopAllBinds()
        {
            rigidbodyVisualizer.RigidbodyEngineData.AuthorityChanged -= AuthorityChanged;
            StopNonAuthoritativeMode();
        }

        private void AuthorityChanged(bool newAuthority)
        {
            if (!newAuthority)
            {
                StartNonAuthoritativeMode();
            }
            else
            {
                StopNonAuthoritativeMode();
            }
        }

        public void FixedUpdate()
        {
            InitializeVelocities();
        }

        private void InitializeVelocities()
        {
            if (!InitialVelocitiesApplied)
            {
                if (rigidbodyVisualizer.RigidbodyEngineData.IsAuthoritativeHere)
                {
                    InitializeAuthoritativeRigidbody();
                }
                else
                {
                    InitializeNonAuthoritativeRigidbody();
                }
            }
        }

        private void InitializeAuthoritativeRigidbody()
        {
            // NOTE: initialVelocitiesJustApplied is used to add a one-frame delay, so that the physics can actually apply the force.
            if (initialVelocitiesJustApplied)
            {
                InitialVelocitiesApplied = true;
                return;
            }
            try
            {
                UpdateSleeping(rigidbodyVisualizer.RigidbodyEngineData.IsSleeping);
                AddForceToReachVelocity(rigidbodyVisualizer.RigidbodyEngineData.Velocity);
                SetAngularVelocity(rigidbodyVisualizer.RigidbodyEngineData.AngularVelocity);
                initialVelocitiesJustApplied = true;
            }
            catch (Exception)
            {
                // NOTE: This is a workaround for an exception that occurs when accessing Rigidbody.velocity.
                // It seems to have some inconsistent internal state (a proper internal unity exception in
                // unmanaged code). We believe that this must be connected with
                // us using adding rigidbodies on just-activated game objects.
            }
        }

        private void InitializeNonAuthoritativeRigidbody()
        {
            InitialVelocitiesApplied = true;
            StartNonAuthoritativeMode();
        }

        private void StartNonAuthoritativeMode()
        {
            if (InitialVelocitiesApplied)
            {
                rigidbodyVisualizer.RigidbodyEngineData.VelocityUpdated += UpdateVelocity;
                rigidbodyVisualizer.RigidbodyEngineData.AngularVelocityUpdated += UpdateAngularVelocity;
                rigidbodyVisualizer.RigidbodyEngineData.IsSleepingUpdated += UpdateSleeping;
            }
        }

        private void StopNonAuthoritativeMode()
        {
            rigidbodyVisualizer.RigidbodyEngineData.VelocityUpdated -= UpdateVelocity;
            rigidbodyVisualizer.RigidbodyEngineData.AngularVelocityUpdated -= UpdateAngularVelocity;
            rigidbodyVisualizer.RigidbodyEngineData.IsSleepingUpdated -= UpdateSleeping;
        }

        private void UpdateSleeping(bool isSleeping)
        {
            if (isSleeping)
            {
                rigidbodyVisualizer.Rigidbody.Sleep();
            }
            else
            {
                rigidbodyVisualizer.Rigidbody.WakeUp();
            }
        }

        private void UpdateVelocity(Improbable.Math.Vector3d velocity)
        {
            if (!rigidbodyVisualizer.Rigidbody.isKinematic)
            {
                AddForceToReachVelocity(velocity);
            }
        }

        private void UpdateAngularVelocity(Improbable.Math.Vector3d angularVelocity)
        {
            if (!rigidbodyVisualizer.Rigidbody.isKinematic)
            {
                SetAngularVelocity(angularVelocity);
            }
        }

        private void SetAngularVelocity(Improbable.Math.Vector3d angularVelocity)
        {
            if (IsSleeping())
            {
                return;
            }
            rigidbodyVisualizer.Rigidbody.AddTorque((angularVelocity.ToUnityVector() - rigidbodyVisualizer.Rigidbody.angularVelocity), ForceMode.VelocityChange);
        }

        private void AddForceToReachVelocity(Improbable.Math.Vector3d velocity)
        {
            if (IsSleeping())
            {
                return;
            }
            rigidbodyVisualizer.Rigidbody.AddForce((velocity.ToUnityVector() - rigidbodyVisualizer.Rigidbody.velocity), ForceMode.VelocityChange);
        }

        private bool IsSleeping()
        {
            return rigidbodyVisualizer.RigidbodyEngineData.IsSleeping;
        }
    }
}