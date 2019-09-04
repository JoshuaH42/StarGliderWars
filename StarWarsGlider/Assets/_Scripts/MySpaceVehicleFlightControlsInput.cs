using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Space
{

    public class MySpaceVehicleFlightControlsInput : VehicleInput {

        float throttleUpInput;
        float throttleDownInput;
        float boostInput;
        bool headTurn = false;
        public Transform cameraHorizontalPivot;
        public Transform cameraVerticalPivot;
        public float headTurnRate = 100.0f;
        private float rotHorizontal = 0f;
        private float rotVertical = 0f;
        private bool alternativeButtonPressed;


        Vector3 throttleSensitivity = new Vector3(1, 1, 1);

        [SerializeField]
        float axisSensitivity = 1f;

        // The rotation, translation and boost inputs that are updated each frame
        private Vector3 rotationInputs = Vector3.zero;
        private Vector3 translationInputs = Vector3.zero;
        private Vector3 boostInputs = Vector3.zero;


        // Reference to the engines component on the current vehicle
        VehicleEngines3D spaceVehicleEngines;

        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The new vehicle.</param>
        /// <returns>Whether the input script succeeded in initializing.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {

            if (!base.Initialize(vehicle)) return false;

            // Clear dependencies
            spaceVehicleEngines = null;

            // Make sure the vehicle has a space vehicle engines component
            spaceVehicleEngines = vehicle.GetComponent<VehicleEngines3D>();
            if (spaceVehicleEngines == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Stop the input.
        /// </summary>
        public override void StopInput()
        {

            base.StopInput();

            // Reset the space vehicle engines to idle
            if (spaceVehicleEngines != null)
            {
                // Set steering to zero
                rotationInputs = Vector3.zero;
                spaceVehicleEngines.SetRotationThrottleValues(rotationInputs);

                // Set movement to zero
                translationInputs = Vector3.zero;
                spaceVehicleEngines.SetTranslationThrottleValues(translationInputs);

                // Set boost to zero
                boostInputs = Vector3.zero;
                spaceVehicleEngines.SetBoostThrottleValues(boostInputs);
            }
        }

        // Do controller steering
        protected void ControllerSteeringUpdate()
        {
            // Pitch inputs
            rotationInputs.x = Input.GetAxis("LStickVertical") * axisSensitivity;

            // Roll, if not in head turn mode or alternative button pressed...
            if (!headTurn && !alternativeButtonPressed)
            {
                rotationInputs.z = Input.GetAxis("RStickHorizontal") * axisSensitivity;
            }

            // ...Head turn mode, if head turn switched on and alternative button not pressed
            else if (headTurn && !alternativeButtonPressed)
            {
                rotHorizontal -= Input.GetAxis("RStickHorizontal") * headTurnRate * Time.deltaTime;
                rotVertical += Input.GetAxis("RStickVertical") * headTurnRate * Time.deltaTime;
                cameraHorizontalPivot.localRotation = Quaternion.Euler(0, rotHorizontal, 0);
                cameraVerticalPivot.localRotation = Quaternion.Euler(rotVertical, 0, 0);
            }

            // Yaw
            rotationInputs.y = Input.GetAxis("LStickHorizontal") * axisSensitivity;

            throttleUpInput = Input.GetAxis("RStickVertical");
            throttleDownInput = Input.GetAxis("RStickVertical");
            boostInput = Input.GetAxis("RButton");
            
        }

        protected void ControllerInput()
        {
            // alternative button pressed?
            if (Input.GetButton("LButton")) alternativeButtonPressed = true;
            else alternativeButtonPressed = false;

            // Activate/deactivate head turn mode
            if (Input.GetButtonDown("rStickButton")){
                // Re-centre camera
                rotHorizontal = 0f;
                rotVertical = 0f;
                cameraHorizontalPivot.localRotation = Quaternion.Euler(0, rotHorizontal, 0);
                cameraVerticalPivot.localRotation = Quaternion.Euler(rotVertical, 0, 0);

                headTurn = !headTurn;
            }


        }




        // Do movement
        protected void MovementUpdate()
        {

            // Forward / backward movement
            translationInputs = spaceVehicleEngines.TranslationThrottleValues;
            if (!headTurn && !alternativeButtonPressed)
            {
                if (throttleUpInput > 0.7f)
                {
                    translationInputs.z += throttleSensitivity.z * Time.deltaTime;
                }
                else if (throttleDownInput < -0.7f)
                {
                    translationInputs.z -= throttleSensitivity.z * Time.deltaTime;
                }
            }

            // Strafe if alternative button pressed
            if (alternativeButtonPressed)
            {
                // Left / right movement
                translationInputs.x = Input.GetAxis("RStickHorizontal") * axisSensitivity;

                // Up / down movement           
                translationInputs.y = Input.GetAxis("RStickVertical") * axisSensitivity;
            }
            else
            {
                translationInputs.x = 0f;
                translationInputs.y = 0f;
            }

            // Boost
            if (boostInput > 0.2f)
            {
                boostInputs = new Vector3(0f, 0f, 1f);
            }
            else if (boostInput < 0.2f)
            {
                boostInputs = Vector3.zero;
            }
        }

        protected override void InputUpdate()
        {
            ControllerInput();

            ControllerSteeringUpdate();

            // Implement steering
            spaceVehicleEngines.SetRotationThrottleValues(rotationInputs);

            MovementUpdate();

            // Implement movement
            spaceVehicleEngines.SetTranslationThrottleValues(translationInputs);

            // Implement boost
            spaceVehicleEngines.SetBoostThrottleValues(boostInputs);

        }

    }
}

