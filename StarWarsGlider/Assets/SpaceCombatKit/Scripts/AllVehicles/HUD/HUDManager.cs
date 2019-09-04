using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// Manages the different components of the HUD for a vehicle.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {

        protected VehicleCamera vehicleCamera;

        protected List<HUDComponent> hudComponents = new List<HUDComponent>();

        protected bool activated = false;

        [Tooltip("Whether to activate the HUD when the scene starts.")]
        [SerializeField]
        protected bool activateOnStart = true;


        protected virtual void Awake()
        {

            hudComponents = new List<HUDComponent>(transform.GetComponentsInChildren<HUDComponent>());

            Vehicle vehicle = transform.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                vehicle.onEntered.AddListener(OnGameAgentEnteredVehicle);
                vehicle.onExited.AddListener(OnGameAgentExitedVehicle);
            }

            vehicleCamera = GameObject.FindObjectOfType<VehicleCamera>();
            if (vehicleCamera != null)
            {
                vehicleCamera.onVehicleCameraViewTargetChanged.AddListener(OnCameraViewChanged);
                for (int i = 0; i < hudComponents.Count; ++i)
                {
                    hudComponents[i].SetCamera(vehicleCamera.MainCamera);
                }
            }
        }
      
        // Called when the scene starts
        protected void Start()
        {
            if (!activated)
            {
                if (activateOnStart)
                {
                    ActivateHUD();
                }
                else
                {
                    DeactivateHUD();
                }
            }  
        }

        /// <summary>
        /// Activate the HUD.
        /// </summary>
        public void ActivateHUD()
        {
            activated = true;

            if (vehicleCamera != null)
            {
                for (int i = 0; i < hudComponents.Count; ++i)
                {
                    hudComponents[i].OnCameraViewChanged(vehicleCamera.SelectedCameraViewTarget);
                }
            }
        }


        /// <summary>
        /// Deactivate the HUD.
        /// </summary>
        public void DeactivateHUD()
        {
            for (int i = 0; i < hudComponents.Count; ++i)
            {
                hudComponents[i].Deactivate();
            }
            activated = false;
        }

     
        /// <summary>
        /// Called when the camera view target changes.
        /// </summary>
        /// <param name="newCameraViewTarget">The new camera view target.</param>
        public void OnCameraViewChanged(CameraViewTarget newCameraViewTarget)
        {
        
            if (!activated) return;

            for (int i = 0; i < hudComponents.Count; ++i)
            {
                hudComponents[i].OnCameraViewChanged(newCameraViewTarget);
            }
        }
        


        protected virtual void OnGameAgentEnteredVehicle(GameAgent agent)
        {
            ActivateHUD();
        }


        protected virtual void OnGameAgentExitedVehicle(GameAgent agent)
        {
            DeactivateHUD();
        }


        public void LateUpdate()
        {
            if (activated)
            {
                for (int i = 0; i < hudComponents.Count; ++i)
                {
                    if (hudComponents[i].Activated)
                    {
                        
                        hudComponents[i].OnUpdateHUD();
                    }
                }
            }
        }
    }
}
