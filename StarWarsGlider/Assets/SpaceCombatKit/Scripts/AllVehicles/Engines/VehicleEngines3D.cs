using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// This class implements engines (movement, steering and boost) for a space vehicle
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleEngines3D : Engines
	{

        // The current translation forces available on each axis (according to power availability etc)
        protected Vector3 availableTranslationForces = Vector3.zero;
        public Vector3 AvailableTranslationForces { get { return availableTranslationForces; } }

        // The current rotation forces available on each axis (according to power availability etc)
        protected Vector3 availableRotationForces = Vector3.zero;
        public Vector3 AvailableRotationForces { get { return availableRotationForces; } }
       
        // The current boost forces available on each axis (according to power availability etc)
        protected Vector3 availableBoostForces = Vector3.zero;
        public Vector3 AvailableBoostForces { get { return availableBoostForces; } }

        [Header("Default Forces")]

        [Tooltip("The default translation (thrust) forces for each axis (used when Power component is not being used).")]
        [SerializeField]
        protected Vector3 defaultTranslationForces = new Vector3(200, 200, 300);

        [Tooltip("The default rotation (steering torque) forces for each axis (used when Power component is not being used).")]
        [SerializeField]
        protected Vector3 defaultRotationForces = new Vector3(8f, 8f, 18f);

        [Tooltip("The default boost forces for each axis (used when Power component is not being used).")]
        [SerializeField]
        protected Vector3 defaultBoostForces = new Vector3(200, 200, 300);

        // 
        [Tooltip("Whether to add the full throttle forces to the boost forces or implement boost forces by themselves.")]
        [SerializeField]
        protected bool applyTranslationForcesDuringBoost = true;

        [Header("Limits")]

        [Tooltip("The translation (thrust) forces limits for each axis. For example, clamping vehicle speed regardless of power settings.")]
        [SerializeField]
        protected Vector3 maxTranslationForces = new Vector3(400, 400, 600);
        public Vector3 MaxTranslationForces { get { return maxTranslationForces; } }

        [Tooltip("The rotation (steering) force limits for each axis. For example, clamping vehicle rotation speed regardless of power settings.")]
        [SerializeField]
        protected Vector3 maxRotationForces = new Vector3(8f, 8f, 18f);
        public Vector3 MaxRotationForces { get { return maxRotationForces; } }

        [SerializeField]
        protected Rigidbody cachedRigidbody;



        /// Called when this component is first added to a gameobject or reset in the inspector
        protected virtual void Reset()
        {
            // Initialize the rigidbody with good values
            cachedRigidbody.useGravity = false;
            cachedRigidbody.mass = 1;
            cachedRigidbody.drag = 3;
            cachedRigidbody.angularDrag = 4;
        }

        private void OnValidate()
        {
            cachedRigidbody = GetComponent<Rigidbody>();
        }


        protected virtual void Awake()
        {
            // Cache the rigidbody
            cachedRigidbody = GetComponent<Rigidbody>();
        }

        // Called when the scene starts
        protected override void Start()
        {
            base.Start();
            
            // Start off with the default forces 
            availableTranslationForces = defaultTranslationForces;
            availableRotationForces = defaultRotationForces;
            availableBoostForces = defaultBoostForces;
        }


        /// <summary>
        /// Get the maximum speed on each axis, for example for loadout data.
        /// </summary>
        /// <param name="withBoost">Whether to include boost in the maximum speed.</param>
        /// <returns>The maximum speed on each axis.</returns>
        public override Vector3 GetDefaultMaxSpeedByAxis(bool withBoost)
		{
            Vector3 maxForces = defaultTranslationForces + (withBoost ? defaultBoostForces : Vector3.zero);
            maxForces = Vector3.Min(maxForces, maxTranslationForces);
            
			return (new Vector3(GetSpeedFromForce(maxForces.x), GetSpeedFromForce(maxForces.y), GetSpeedFromForce(maxForces.z)));

		}

        /// <summary>
        /// Get the current maximum speed on each axis, for example for normalizing speed indicators.
        /// </summary>
        /// <param name="withBoost">Whether to include boost in the maximum speed.</param>
        /// <returns>The maximum speed on each axis.</returns>
        public override Vector3 GetCurrentMaxSpeedByAxis(bool withBoost)
        {
            Vector3 maxForces = availableTranslationForces + (withBoost ? availableBoostForces : Vector3.zero);
            maxForces = Vector3.Min(maxForces, maxTranslationForces);

            return (new Vector3(GetSpeedFromForce(maxForces.x), GetSpeedFromForce(maxForces.y), GetSpeedFromForce(maxForces.z)));

        }


        /// <summary>
        /// Calculate the maximum speed of this Rigidbody for a given force.
        /// </summary>
        /// <param name="force">The linear force to be used in the calculation.</param>
        /// <returns>The maximum speed.</returns>
        protected virtual float GetSpeedFromForce(float force)
		{
			float delta_V_Thrust = (force / cachedRigidbody.mass) * Time.fixedDeltaTime;
			float dragFactor = Time.fixedDeltaTime * cachedRigidbody.drag;
			float maxSpeed = delta_V_Thrust / dragFactor;

			return maxSpeed;
		}

        // Apply the physics
        protected virtual void FixedUpdate()
		{
            if (enginesActivated)
            {

                // Implement steering torques
                cachedRigidbody.AddRelativeTorque(Vector3.Scale(steeringValues, availableRotationForces), ForceMode.Acceleration);

                Vector3 nextTranslationThrottleValues = translationThrottleValues;

                // If full throttle is to be applied during boost, add it
                if (applyTranslationForcesDuringBoost)
                {
                    if (boostThrottleValues.x > 0.5f)
                        nextTranslationThrottleValues.x = Mathf.Clamp(Mathf.Sign(nextTranslationThrottleValues.x), minTranslationThrottleValues.x, maxTranslationThrottleValues.x);
                    if (boostThrottleValues.y > 0.5f)
                        nextTranslationThrottleValues.y = Mathf.Clamp(Mathf.Sign(nextTranslationThrottleValues.y), minTranslationThrottleValues.y, maxTranslationThrottleValues.y);
                    if (boostThrottleValues.z > 0.5f)
                        nextTranslationThrottleValues.z = Mathf.Clamp(1, minTranslationThrottleValues.z, maxTranslationThrottleValues.z);

                }
                
                // Get next forces to be applied
                Vector3 nextForces = Vector3.Scale(nextTranslationThrottleValues, availableTranslationForces) +
                                        Vector3.Scale(boostThrottleValues, availableBoostForces);

                // Clamp forces within limits
                nextForces = Vector3.Min(nextForces, maxTranslationForces);

                // Implement forces
                cachedRigidbody.AddRelativeForce(nextForces);
            }

		}
	}
}
