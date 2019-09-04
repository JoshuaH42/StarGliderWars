using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class PlayerCameraViewInput : VehicleInput
    {

        [Header("Camera View")]

        [SerializeField]
        protected VehicleCamera vehicleCamera;

        [SerializeField]
        protected CustomInput cycleViewForwardInput;

        [SerializeField]
        protected CustomInput cycleViewBackwardInput;

        [Header("Interior Look Around")]

        [SerializeField]
        protected GimbalController cameraGimbal;

        [SerializeField]
        protected float cameraGimbalRotationSpeed = 1;

        [SerializeField]
        protected CustomInput enableLookAroundInput;

        [SerializeField]
        protected CustomInput horizontalRotationAxisInput;

        [SerializeField]
        protected CustomInput verticalRotationAxisInput;


        protected override void InputUpdate()
        {
            if (vehicleCamera != null)
            {
                if (cycleViewForwardInput.Down())
                {
                    vehicleCamera.CycleCameraView(true);
                    if (cameraGimbal != null) cameraGimbal.ResetGimbal();
                }
                else if (cycleViewBackwardInput.Down())
                {
                    vehicleCamera.CycleCameraView(false);
                    if (cameraGimbal != null) cameraGimbal.ResetGimbal();
                }
            }

            if (vehicleCamera.HasCameraViewTarget && vehicleCamera.SelectedCameraViewTarget.CameraView == VehicleCameraView.Interior)
            {
                if (cameraGimbal != null)
                {
                    if (enableLookAroundInput.Pressed())
                    {
                        // Look around
                        cameraGimbal.Rotate(new Vector2(cameraGimbalRotationSpeed * horizontalRotationAxisInput.FloatValue(),
                                                        cameraGimbalRotationSpeed * -verticalRotationAxisInput.FloatValue()));
                    }

                    if (enableLookAroundInput.Up())
                    {
                        cameraGimbal.ResetGimbal();
                    }
                    
                }
            }
        }
    }
}
