using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Base class for a script that controls the camera for a specific type of vehicle.
    /// </summary>
    public class VehicleCameraController : MonoBehaviour
    {

        [Header("General")]

        [Tooltip("The class of the vehicle that this camera controller is for.")]
        [SerializeField]
        protected VehicleClass vehicleClass;
        public virtual VehicleClass VehicleClass { get { return vehicleClass; } }

        // A reference to the vehicle camera
        protected VehicleCamera vehicleCamera;
        public VehicleCamera VehicleCamera { set { vehicleCamera = value; } }

        [Header("Starting Values")]

        [Tooltip("The camera view that is shown upon entering the vehicle.")]
        [SerializeField]
        protected VehicleCameraView startingView;

        [Tooltip("Whether to default to the first available view, if the startingView value is not set.")]
        [SerializeField]
        protected bool defaultToFirstAvailableView = true;

        // Whether this camera controller is currently activated
        protected bool controllerActive = false;
        public bool ControllerActive { get { return controllerActive; } }

        // Whether this camera controller is ready to be activated
        protected bool initialized = false;
        public bool Initialized { get { return initialized; } }

        
        /// <summary>
        /// Called to activate this camera controller (for example when the Vehicle Camera's target vehicle changes).
        /// </summary>
        public virtual void StartController()
        {
            // If this camera controller is ready, activate it.
            if (initialized) controllerActive = true;  
        }


        /// <summary>
        /// Called to deactivate this camera controller.
        /// incompatible vehicle.
        /// </summary>
        public virtual void StopController()
        {
            controllerActive = false;
        }

        /// <summary>
        /// Set the target vehicle for this camera controller.
        /// </summary>
        /// <param name="vehicle">The target vehicle.</param>
        /// <param name="startController">Whether to start the controller immediately.</param>
        public virtual void SetVehicle(Vehicle vehicle, bool startController)
        {

            StopController();

            initialized = false;

            if (vehicle == null) return;

            if (Initialize(vehicle))
            {
                initialized = true;
                vehicleCamera.SetView(startingView);
                if (startController) StartController();
            }
        }


        /// <summary>
        /// Initialize this camera controller with a vehicle.
        /// </summary>
        /// <param name="vehicle">Whether the camera controller succeeded in initializing.</param>
        /// <returns></returns>
        protected virtual bool Initialize(Vehicle vehicle)
        {          
            // Check if the vehicle class matches the camera controller class 
            if (vehicle.VehicleClass == this.vehicleClass)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}