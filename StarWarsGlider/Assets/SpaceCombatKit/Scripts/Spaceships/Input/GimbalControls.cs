﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class GimbalControls : VehicleInput
    {

        protected GimbalController gimbalController;

        [SerializeField]
        protected float gimbalRotationSpeed = 100;

        [SerializeField]
        protected CustomInput horizontalRotationInputAxis = new CustomInput("Gimballed Vehicles", "Look Horizontal", "Mouse X");

        [SerializeField]
        protected CustomInput verticalRotationInputAxis = new CustomInput("Gimballed Vehicles", "Look Vertical", "Mouse Y");



        protected override bool Initialize(Vehicle vehicle)
        {
            if(!base.Initialize(vehicle)) return false;

            gimbalController = vehicle.GetComponent<GimbalController>();

            return (gimbalController != null);

        }

        protected override void InputUpdate()
        {
            base.InputUpdate();

            gimbalController.Rotate(new Vector2(horizontalRotationInputAxis.FloatValue() * gimbalRotationSpeed * Time.deltaTime,
                                            -verticalRotationInputAxis.FloatValue() * gimbalRotationSpeed * Time.deltaTime));
        }

    }
}