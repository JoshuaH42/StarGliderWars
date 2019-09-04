using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Input script for controlling the steering and movement of a space fighter vehicle.
    /// </summary>
    public class PlayerSpaceVehicleFlightControls : VehicleInput
    {
        [Header("Control Scheme")]

        [SerializeField]
        protected bool linkYawAndRoll = false;

        [SerializeField]
        protected float yawRollRatio = 1;

        [Header("Mouse Steering")]

        [SerializeField]
        protected bool mouseSteeringEnabled = true;

        [SerializeField]
        protected bool mouseVerticalInverted = false;

        [SerializeField]
        protected float mousePitchSensitivity = 1;

        [SerializeField]
        protected float mouseYawSensitivity = 1;

        [SerializeField]
        protected float mouseRollSensitivity = 1;

        [SerializeField]
        protected float mouseDeadRadius = 0;

        // Flag for enabling/disabling the mouse input in this script 
        protected bool mouseInputEnabled = true;

        [Header("Keyboard Steering")]

        [SerializeField]
        protected bool keyboardVerticalInverted = false;

        protected bool steeringEnabled = true;

        [SerializeField]
        protected CustomInput pitchAxisInput = new CustomInput("Flight Controls", "Pitch", "Vertical");

        [SerializeField]
        protected CustomInput yawAxisInput = new CustomInput("Flight Controls", "Yaw", "Horizontal");

        [SerializeField]
        protected CustomInput rollAxisInput = new CustomInput("Flight Controls", "Roll", "Roll");

        [Header("Throttle")]

        [SerializeField]
        protected CustomInput throttleUpInput = new CustomInput("Flight Controls", "Throttle Up", KeyCode.Z);

        [SerializeField]
        protected CustomInput throttleDownInput = new CustomInput("Flight Controls", "Throttle Down", KeyCode.X);

        [SerializeField]
        protected CustomInput strafeVerticalInput = new CustomInput("Flight Controls", "Strafe Vertical", "Strafe Vertical");

        [SerializeField]
        protected CustomInput strafeHorizontalInput = new CustomInput("Flight Controls", "Strafe Horizontal", "Strafe Horizontal");

        [SerializeField]
        protected Vector3 throttleSensitivity = new Vector3(1, 1, 1);

        protected bool movementEnabled = true;

        // The rotation, translation and boost inputs that are updated each frame
        protected Vector3 rotationInputs = Vector3.zero;
        protected Vector3 translationInputs = Vector3.zero;
        protected Vector3 boostInputs = Vector3.zero;

        [Header("Boost")]

        [SerializeField]
        protected CustomInput boostInput = new CustomInput("Flight Controls", "Boost", KeyCode.Tab);

        // Reference to the engines component on the current vehicle
        protected VehicleEngines3D spaceVehicleEngines;
        


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

        /// <summary>
        /// Enable steering input.
        /// </summary>
        public virtual void EnableSteering()
        {
            steeringEnabled = true;
        }


        /// <summary>
        /// Disable steering input.
        /// </summary>
        /// <param name="clearCurrentValues">Whether to clear current steering values.</param>
        public virtual void DisableSteering(bool clearCurrentValues)
        {
            steeringEnabled = false;

            if (clearCurrentValues)
            {
                // Set steering to zero
                rotationInputs = Vector3.zero;
                spaceVehicleEngines.SetRotationThrottleValues(rotationInputs);
            }
        }


        /// <summary>
        /// Enable movement input.
        /// </summary>
        public virtual void EnableMovement()
        {
            movementEnabled = true;
        }

        /// <summary>
        /// Disable the movement input.
        /// </summary>
        /// <param name="clearCurrentValues">Whether to clear current throttle values.</param>
        public virtual void DisableMovement(bool clearCurrentValues)
        {
            movementEnabled = false;

            if (clearCurrentValues)
            {
                // Set movement to zero
                translationInputs = Vector3.zero;
                spaceVehicleEngines.SetTranslationThrottleValues(translationInputs);

                // Set boost to zero
                boostInputs = Vector3.zero;
                spaceVehicleEngines.SetBoostThrottleValues(boostInputs);
            }
        }


        /// <summary>
        /// Called to toggle the mouse input in this script.
        /// </summary>
        /// <param name="setEnabled">Whether the mouse input is to be enabled.</param>
        public virtual void SetMouseInputEnabled(bool setEnabled)
        {
            mouseInputEnabled = setEnabled;
        }


        // Do mouse steering
        protected virtual void MouseSteeringUpdate()
        {
            // Get the mouse position in viewport space with 0,0 at the center of the screen
            Vector3 mousePos = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0) - new Vector3(0.5f, 0.5f, 0f);
            mousePos = mousePos * 2;    // Go from -1 to 1 left to right of screen

            // Adjust the mouse distance taking into account the dead radius
            float mouseDist = Vector3.Magnitude(mousePos);
            mouseDist = Mathf.Max(mouseDist - mouseDeadRadius, 0);
            mousePos = mousePos.normalized * mouseDist;

            // Update pitch
            rotationInputs.x = Mathf.Clamp((mouseVerticalInverted ? 1 : -1) * mousePos.y * mousePitchSensitivity, -1f, 1f);

            // Linked yaw and roll
            if (linkYawAndRoll)
            {
                rotationInputs.z = Mathf.Clamp(-mousePos.x * mouseRollSensitivity, -1f, 1f);
                rotationInputs.y = Mathf.Clamp(-rotationInputs.z * yawRollRatio, -1f, 1f);
            }
            // Separate axes
            else
            {
                // Roll
                rotationInputs.z = rollAxisInput.FloatValue();

                // Yaw
                rotationInputs.y = Mathf.Clamp(mousePos.x * mouseYawSensitivity, -1f, 1f);
            }
        }


        // Do keyboard steering
        protected virtual void KeyboardSteeringUpdate()
        {
            // Pitch
            rotationInputs.x = (keyboardVerticalInverted ? -1 : 1) * pitchAxisInput.FloatValue();

            // Linked yaw and roll
            if (linkYawAndRoll)
            {
                rotationInputs.z = -yawAxisInput.FloatValue();
                rotationInputs.y = Mathf.Clamp(-rotationInputs.z * yawRollRatio, -1f, 1f);
            }
            // Separate axes
            else
            {
                // Roll
                rotationInputs.z = rollAxisInput.FloatValue();

                // Yaw
                rotationInputs.y = yawAxisInput.FloatValue();
            }
        }


        // Do movement
        protected virtual void MovementUpdate()
        {
            // Forward / backward movement
            translationInputs = spaceVehicleEngines.TranslationThrottleValues;
            if (throttleUpInput.Pressed())
            {
                translationInputs.z += throttleSensitivity.z * Time.deltaTime;
            }
            else if (throttleDownInput.Pressed())
            {
                translationInputs.z -= throttleSensitivity.z * Time.deltaTime;
            }

            // Left / right movement
            translationInputs.x = strafeHorizontalInput.FloatValue();

            // Up / down movement
            translationInputs.y = strafeVerticalInput.FloatValue();

            // Boost
            if (boostInput.Down())
            {
                boostInputs = new Vector3(0f, 0f, 1f);
            }
            else if (boostInput.Up())
            {
                boostInputs = Vector3.zero;
            }
        }


        protected override void InputUpdate()
        {

            if (steeringEnabled)
            {
                // Do mouse or keyboard steering
                if (mouseInputEnabled && mouseSteeringEnabled)
                {
                    MouseSteeringUpdate();
                }
                else
                {
                    KeyboardSteeringUpdate();
                }

                // Implement steering
                spaceVehicleEngines.SetRotationThrottleValues(rotationInputs);
            }
            
            if (movementEnabled)
            {
                // Update movement
                MovementUpdate();

                // Implement movement
                spaceVehicleEngines.SetTranslationThrottleValues(translationInputs);

                // Implement boost
                spaceVehicleEngines.SetBoostThrottleValues(boostInputs);
            }
        }
    }
}