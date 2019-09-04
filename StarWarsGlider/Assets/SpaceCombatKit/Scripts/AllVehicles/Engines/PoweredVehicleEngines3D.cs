using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class PoweredVehicleEngines3D : VehicleEngines3D
    {

        [Header("Power")]

        [Tooltip("The Power component on this vehicle.")]
        [SerializeField]
        protected Power power;

        [Tooltip("The coefficients that are multiplied by the available 'direct' power to the engines to determine the rotation (steering) forces.")]
        [SerializeField]
        protected Vector3 powerToRotationForceCoefficients = new Vector3(0.1f, 0.1f, 0.2f);

        [Tooltip("The coefficients that are multiplied by the available 'direct' power to the engines to determine the translation (thrust) forces.")]
        [SerializeField]
        protected Vector3 powerToTranslationForceCoefficients = new Vector3(0.1f, 0.1f, 0.2f);


        protected override void Reset()
        {
            base.Reset();
            power = GetComponent<Power>();
        }

        private void Update()
        {

            if (power == null) return;

            // Calculate the current available pitch, yaw and roll torques
            if (power.GetPowerConfiguration(PoweredSubsystemType.Engines) != SubsystemPowerConfiguration.Unpowered)
            {
                availableRotationForces = power.GetSubsystemTotalPower(PoweredSubsystemType.Engines) * powerToRotationForceCoefficients;
            }
            else
            {
                availableRotationForces = defaultRotationForces;
            }

            // Clamp below maximum limits
            availableRotationForces.x = Mathf.Min(availableRotationForces.x, maxRotationForces.x);
            availableRotationForces.y = Mathf.Min(availableRotationForces.y, maxRotationForces.y);
            availableRotationForces.z = Mathf.Min(availableRotationForces.z, maxRotationForces.z);

            // Calculate the currently available thrust
            if (power.GetPowerConfiguration(PoweredSubsystemType.Engines) != SubsystemPowerConfiguration.Unpowered)
            {
                availableTranslationForces = power.GetSubsystemTotalPower(PoweredSubsystemType.Engines) * powerToTranslationForceCoefficients;
            }
            else
            {
                availableTranslationForces = defaultTranslationForces;
            }

            // Keep the thrust below the maximum limit
            availableTranslationForces.x = Mathf.Min(availableTranslationForces.x, maxTranslationForces.x);
            availableTranslationForces.y = Mathf.Min(availableTranslationForces.y, maxTranslationForces.y);
            availableTranslationForces.z = Mathf.Min(availableTranslationForces.z, maxTranslationForces.z);
            
        }
    }
}