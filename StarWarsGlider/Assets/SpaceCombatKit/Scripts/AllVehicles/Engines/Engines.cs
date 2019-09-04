using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace VSX.UniversalVehicleCombat 
{

	/// <summary>
    /// Base class for engines vehicle engines.
    /// </summary>
	public class Engines : MonoBehaviour
	{

        // Engine on/off
        protected bool enginesActivated = false;
        public bool EnginesActivated { get { return enginesActivated; } }

        [Tooltip("Whether to activate the engines when the scene starts.")]
        [SerializeField]
        protected bool activateEnginesAtStart = true;

        [Header("Current Settings")]

        [Tooltip("The current steering rotation input values (-1 to 1) for each axis.")]
        protected Vector3 steeringValues;
        public Vector3 SteeringValues { get { return steeringValues; } }

        [Tooltip("The current translation (movement) input values (-1 to 1) for each axis.")]
        [SerializeField]
        protected Vector3 translationThrottleValues;
        public Vector3 TranslationThrottleValues { get { return translationThrottleValues; } }

        [Tooltip("The current boost input values (-1 to 1) for each axis.")]
        [SerializeField]
        protected Vector3 boostThrottleValues;
        public Vector3 BoostThrottleValues { get { return boostThrottleValues; } }

        [Header("Input Limits")]

        // The minimum translation throttle settings for each axis (i.e. the maximum input values along the negative axis)
        [Tooltip("The minimum throttle settings (>= -1) for each axis (e.g. to limit reverse speed).")]
        [SerializeField]
        protected Vector3 minTranslationThrottleValues = new Vector3(-1, -1, -0.1f);
        public Vector3 MinTranslationThrottleValues { get { return minTranslationThrottleValues; } }

        // The maximum translation throttle settings for each axis 
        [Tooltip("The maximum throttle settings for each axis (in the positive direction).")]
        [SerializeField]
        protected Vector3 maxTranslationThrottleValues = new Vector3(1f, 1f, 1f);
        public Vector3 MaxTranslationThrottleValues { get { return maxTranslationThrottleValues; } }

        [Tooltip("Disable steering and throttle.")]
        [SerializeField]
        protected bool controlsDisabled = false;
        public bool ControlsDisabled
        {
            get { return controlsDisabled; }
            set { controlsDisabled = value; }
        }


        // Called when scene starts
        protected virtual void Start()
        {
            // Turn the engine on or off at start
            if (activateEnginesAtStart)
            {
                SetEngineActivation(true);
            }
        }

        /// <summary>
        /// Turn the engine on or off.
        /// </summary>
        /// <param name="setActivated">Whether the engine should be activated or deactivated.</param>
        public virtual void SetEngineActivation(bool setActivated)
        {
            enginesActivated = setActivated;

            if (!enginesActivated)
            {
                steeringValues = Vector3.zero;
                translationThrottleValues = Vector3.zero;
            }
        }

        public void SetTranslationThrottleLimits(Vector3 minTranslationThrottleValues, Vector3 maxTranslationThrottleValues)
        {
            this.minTranslationThrottleValues = minTranslationThrottleValues;
            this.maxTranslationThrottleValues = maxTranslationThrottleValues;
        }

		/// <summary>
		/// Set the translation (movement) throttle values.
		/// </summary>
		/// <param name="newValuesByAxis"> New values by axis. </param>
		public virtual void SetTranslationThrottleValues(Vector3 newValuesByAxis)
        {
            
            if (controlsDisabled) return;
            
            // Set and clamp the translation throttle values
            translationThrottleValues.x = Mathf.Clamp(newValuesByAxis.x, minTranslationThrottleValues.x, maxTranslationThrottleValues.x);
            translationThrottleValues.y = Mathf.Clamp(newValuesByAxis.y, minTranslationThrottleValues.y, maxTranslationThrottleValues.y);
            translationThrottleValues.z = Mathf.Clamp(newValuesByAxis.z, minTranslationThrottleValues.z, maxTranslationThrottleValues.z);

        }


        /// <summary>
        /// Increase/decrease translation (movement) throttle values.
        /// </summary>
        /// <param name="incrementationAmountsByAxis"> Incrementation amounts by axis. </param>
        public virtual void IncrementTranslationThrottleValues(Vector3 incrementationAmountsByAxis)
        {

            if (controlsDisabled) return;

            // Update the translation throttle values
            translationThrottleValues += incrementationAmountsByAxis;

            // Clamp the translation throttle values
            translationThrottleValues.x = Mathf.Clamp(translationThrottleValues.x, minTranslationThrottleValues.x, maxTranslationThrottleValues.x);
            translationThrottleValues.y = Mathf.Clamp(translationThrottleValues.y, minTranslationThrottleValues.y, maxTranslationThrottleValues.y);
            translationThrottleValues.z = Mathf.Clamp(translationThrottleValues.z, minTranslationThrottleValues.z, maxTranslationThrottleValues.z);
            
        }


        /// <summary>
        /// Set the rotation throttle values.
        /// </summary>
        /// <param name="newValuesByAxis"> New values by axis. </param>
        public virtual void SetRotationThrottleValues(Vector3 newValuesByAxis)
        {

            if (controlsDisabled) return;

            // Set and clamp the rotation throttle values 
            steeringValues.x = Mathf.Clamp(newValuesByAxis.x, -1f, 1f);
            steeringValues.y = Mathf.Clamp(newValuesByAxis.y, -1f, 1f);
            steeringValues.z = Mathf.Clamp(newValuesByAxis.z, -1f, 1f);
            
        }


		/// <summary>
		/// Increase/decrease rotation throttle values.
		/// </summary>
		/// <param name="incrementationRatesByAxis">Rotation throttle incrementation amounts by axis.</param>
		public virtual void IncrementRotationThrottleValues (Vector3 incrementationAmountsByAxis)
        {

            if (controlsDisabled) return;

            // Update the rotation throttle values 
            steeringValues += incrementationAmountsByAxis;

            // Clamp the rotation throttle values
            steeringValues.x = Mathf.Clamp(steeringValues.x, -1f, 1f);
            steeringValues.y = Mathf.Clamp(steeringValues.y, -1f, 1f);
            steeringValues.z = Mathf.Clamp(steeringValues.z, -1f, 1f);
            
        }

	
		/// <summary>
		/// Called by input script for setting boost throttle values.
		/// </summary>
		/// <param name="newValues">New values by axis.</param>
		public virtual void SetBoostThrottleValues (Vector3 newValuesByAxis)
        {

            if (controlsDisabled) return;

            // Set and clamp the boost throttle values
            boostThrottleValues.x = Mathf.Clamp(newValuesByAxis.x, -1f, 1f);
            boostThrottleValues.y = Mathf.Clamp(newValuesByAxis.y, -1f, 1f);
            boostThrottleValues.z = Mathf.Clamp(newValuesByAxis.z, -1f, 1f);

        }

        /// <summary>
        /// Clear all the rotation, translation and boost throttle settings.
        /// </summary>
        public virtual void ClearAllInputs()
        {
            translationThrottleValues = Vector3.zero;
            steeringValues = Vector3.zero;
            boostThrottleValues = Vector3.zero;
        }


        /// <summary>
        /// Get the default maximum speed on each axis, for example for loadout data.
        /// </summary>
        /// <param name="withBoost">Whether to include boost in the maximum speed.</param>
        /// <returns>The maximum speed on each axis.</returns>
        public virtual Vector3 GetDefaultMaxSpeedByAxis(bool withBoost)
        {
            // Override in derived classes.
            return Vector3.zero;
        }

        /// <summary>
        /// Get the current maximum speed on each axis, for example for normalizing speed indicators.
        /// </summary>
        /// <param name="withBoost">Whether to include boost in the maximum speed.</param>
        /// <returns>The current maximum speed on each axis.</returns>
        public virtual Vector3 GetCurrentMaxSpeedByAxis(bool withBoost)
        {
            // Override in derived classes.
            return Vector3.zero;
        }
    }
}
