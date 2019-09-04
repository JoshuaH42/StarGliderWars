using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class CapitalShipCameraController : VehicleCameraController
    {

        [Header("Boost Effects")]

        [SerializeField]
        private float boostFieldOfView = 80;

        [SerializeField]
        private float boostFieldOfViewLerpSpeed = 0.1f;

        public override VehicleClass VehicleClass { get { return VehicleClass.CapitalShip; } }

        protected VehicleEngines3D engines;


        protected void Reset()
        {
            vehicleClass = VehicleClass.CapitalShip;
        }

        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            engines = vehicle.GetComponent<VehicleEngines3D>();

            return true;
        }


        void LateUpdate()
        {

            if (!controllerActive) return;
           
            // Positioning of the locked interior camera must be done in late update to make sure that there is no position lag, so that the 
            // aiming reticle lines up with the camera forward vector			
            if (vehicleCamera.SelectedCameraViewTarget != null)
            {
                if (vehicleCamera.SelectedCameraViewTarget.LockPosition)
                {
                    vehicleCamera.transform.position = vehicleCamera.SelectedCameraViewTarget.transform.position;
                }
                if (vehicleCamera.SelectedCameraViewTarget.LockRotation)
                {
                    vehicleCamera.transform.rotation = vehicleCamera.SelectedCameraViewTarget.transform.rotation;
                }
            }
        }


        protected virtual void Update()
        {
            
            if (!controllerActive) return;

            if (engines != null)
            {
                float targetFOV = engines.BoostThrottleValues.z * boostFieldOfView +
                                    (1 -engines.BoostThrottleValues.z) * vehicleCamera.DefaultFieldOfView;

                vehicleCamera.MainCamera.fieldOfView = Mathf.Lerp(vehicleCamera.MainCamera.fieldOfView, targetFOV, boostFieldOfViewLerpSpeed);

            }
        }
    }
}
