using System;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class PlayerFirstPersonCharacterInput : VehicleInput
    {

        protected FirstPersonCharacterController characterController;

        [SerializeField]
        protected CustomInput walkForwardBackwardAxisInput = new CustomInput("First Person Character", "Walk", "Vertical");

        [SerializeField]
        protected CustomInput strafeHorizontalAxisInput = new CustomInput("First Person Character", "Strafe", "Horizontal");

        [SerializeField]
        protected CustomInput runInput = new CustomInput("First Person Character", "Run", KeyCode.LeftShift);

        [SerializeField]
        protected CustomInput jumpInput = new CustomInput("First Person Character", "Jump", KeyCode.Space);


        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <returns>Whether initialization succeeded</returns>
        protected override bool Initialize(Vehicle vehicle)
        {

            characterController = vehicle.GetComponent<FirstPersonCharacterController>();
            if (characterController == null)
            {
                return false;
            }
           
            return true;
        }

        public override void StartInput()
        {
            base.StartInput();
        }

        public override void StopInput()
        {
            base.StopInput();
        }

        // Update is called once per frame
        protected override void InputUpdate()
        {

            // Moving
            float horizontal = strafeHorizontalAxisInput.FloatValue();
            float vertical = walkForwardBackwardAxisInput.FloatValue();
            characterController.SetMovementInputs(horizontal, 0, vertical);

            // Jumping
            if (jumpInput.Down())
            {
                characterController.Jump();
            }

#if !MOBILE_INPUT
            characterController.SetRunning(runInput.Pressed());
#endif

        }
    }
}
