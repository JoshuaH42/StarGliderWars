using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Base class for vehicle input components.
    /// </summary>
    public class VehicleInput : MonoBehaviour
    {

        [Header("Vehicle Input")]

        // Vehicle to control when the scene starts
        [SerializeField]
        protected Vehicle startingVehicle;

        [SerializeField]
        protected bool specifyCompatibleGameStates = false;

        [SerializeField]
        protected List<GameState> compatibleGameStates = new List<GameState>();

        [SerializeField]
        protected bool specifyCompatibleVehicleClasses = false;

        [SerializeField]
        protected List<VehicleClass> compatibleVehicleClasses = new List<VehicleClass>();

        // Whether this input component has everything it needs to run
        protected bool initialized = false;
        public bool Initialized { get { return initialized; } }
 
        // Whether this input component is currently activated
        protected bool inputActive;
        public virtual bool InputActive { get { return inputActive; } }


        protected virtual void Start()
        {
            // Initialize with the starting vehicle
            if (startingVehicle != null)
            {
                SetVehicle(startingVehicle, true);
            }
        }

        /// <summary>
        /// Start running this input script.
        /// </summary>
        public virtual void StartInput()
        {
            if (initialized) inputActive = true;
        }

        /// <summary>
        /// Stop running this input script.
        /// </summary>
        public virtual void StopInput()
        {
            inputActive = false;
        }

        /// <summary>
        /// Set a new vehicle for the input component.
        /// </summary>
        /// <param name="vehicle">The new vehicle</param>
        /// <param name="startInput">Whether to start input if initialization is successful.</param>
        public virtual void SetVehicle(Vehicle vehicle, bool startInput)
        {

            StopInput();

            initialized = false;

            if (vehicle == null) return;
            
            if (Initialize(vehicle))
            {
                initialized = true;
                if (startInput) StartInput();
            }
        }

        /// <summary>
        /// Attempt to initialize the input component with a vehicle reference.
        /// </summary>
        /// <param name="vehicle">The vehicle to attempt initialization with.</param>
        /// <returns> Whether initialization was successful. </returns>
        protected virtual bool Initialize(Vehicle vehicle)
        {
            if (specifyCompatibleVehicleClasses)
            {
                for (int i = 0; i < compatibleVehicleClasses.Count; ++i)
                {
                    if (compatibleVehicleClasses[i] == vehicle.VehicleClass)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
           
            return false;
        }

        /// <summary>
        /// Put all your input code in an override of this method.
        /// </summary>
        protected virtual void InputUpdate() { }


        protected virtual void Update()
        {
            if (inputActive)
            {
                if (specifyCompatibleGameStates && GameStateManager.Instance != null)
                {
                    for (int i = 0; i < compatibleGameStates.Count; ++i)
                    {
                        if (compatibleGameStates[i] == GameStateManager.Instance.CurrentGameState)
                        {
                            InputUpdate();
                            break;
                        }
                    }
                }
                else
                {
                    InputUpdate();
                }
            }
        }
    }
}