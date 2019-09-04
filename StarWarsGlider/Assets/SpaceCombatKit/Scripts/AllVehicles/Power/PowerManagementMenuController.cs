﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace VSX.UniversalVehicleCombat
{

	/// <summary>
    /// This class manages the power management menu, where the player can see and modify how the power is distributed 
    /// between subsystems.
    /// </summary>
	public class PowerManagementMenuController : SimpleMenuManager 
	{

		[Header ("General")]

        [SerializeField]
        protected Power power;

		[SerializeField]
		private Text totalPowerText;
		
		[Header ("Power Adjustment")]

		[SerializeField]
		private Transform powerBall;

		[SerializeField]
		private RectTransform triangle;

        [SerializeField]
        private float triangleFractionOfRectHeight = 0.75f;


		[Header ("Power Indicators")]

		[SerializeField]
		private SubsystemPowerInfoController enginesInfoController;

		[SerializeField]
		private SubsystemPowerInfoController weaponsInfoController;

		[SerializeField]
		private SubsystemPowerInfoController shieldsInfoController;


        [Header("Triangle Slider")]

        [SerializeField]
        private float powerAdjustSpeed = 1;

		Vector3 enginesApexPos;

		Vector3 powerBallPosition;

		float enginesFraction;
		float weaponsFraction;
		float shieldsFraction;
        
		

		protected override void Awake()
		{

            base.Awake();

			// Calculate the necessary triangle parameters

			float sideLength = 1f/(Mathf.Sin(60f*Mathf.Deg2Rad));

			enginesApexPos = new Vector3(-sideLength/2f, 1f/3f, 0f);
			
			enginesFraction = 1f/3f;
			weaponsFraction = 1f/3f;
			shieldsFraction = 1f/3f;

			powerBallPosition = Vector3.zero;

        }

        void Start()
		{
			// Initialize the power distribution
			RecalculatePower();
		}
	
	
		/// <summary>
        /// Activate the menu.
        /// </summary>
        /// <returns>Whether the menu was successfully activated.</returns>
		public override void ActivateMenu()
		{

            if (power == null) return;

            base.ActivateMenu();

			RecalculatePower();
			
		}

		
		/// <summary>
        /// Event called when the focused vehicle changes.
        /// </summary>
        /// <param name="newVehicle"></param>
		public void OnFocusedVehicleChanged(Vehicle newVehicle)
		{
			if (newVehicle == null)
            {
                power = null;
            }
            else
            {
                power = newVehicle.GetComponent<Power>();
            }
		}


        /// <summary>
        /// Move the power ball horizontally.
        /// </summary>
        /// <param name="amount">The amount to move.</param>
		public void MovePowerBallHorizontally (float amount)
        {
            
            if (!menuActivated) return;
            
            powerBallPosition += amount * Vector3.right * powerAdjustSpeed * Time.unscaledDeltaTime;

            //ClampInsideTriangle();
            RecalculatePower();

        }

        /// <summary>
        /// Move the power ball vertically.
        /// </summary>
        /// <param name="amount">The amount to move.</param>
		public void MovePowerBallVertically(float amount)
        {

            if (!menuActivated) return;

            powerBallPosition += amount * Vector3.up * powerAdjustSpeed * Time.unscaledDeltaTime;
            
            ClampInsideTriangle();
            RecalculatePower();

        }


        /// <summary>
        /// Update the power distribution based on the power triangle setting.
        /// </summary>
		void RecalculatePower()
		{

			if (power == null) return;
			
			powerBall.GetComponent<RectTransform>().localPosition = powerBallPosition * (triangle.sizeDelta.y * triangleFractionOfRectHeight);
			
			// Calculate the shield (bottom point) fraction 
			shieldsFraction = Mathf.Abs(powerBallPosition.y - (1f/3f)); // The distance below midpoint minus the length of triangle above midpoint
			
			// Calculate the engines (top left) fraction
			float oppositeSideSlope = Mathf.Tan(60f * Mathf.Deg2Rad);		// Get the slope of the side opposite the engine apex
			float yInt = powerBallPosition.y - oppositeSideSlope * powerBallPosition.x; // Get the y intercept of the line that passes through the powerball pos
			float distToLine = DistToLine(enginesApexPos.x, enginesApexPos.y, oppositeSideSlope, yInt);
			enginesFraction = 1-distToLine;
			weaponsFraction = 1-(shieldsFraction + enginesFraction);
			
			power.SetSubsystemDistributablePowerFraction (PoweredSubsystemType.Engines, enginesFraction);
			power.SetSubsystemDistributablePowerFraction (PoweredSubsystemType.Weapons, weaponsFraction);
			power.SetSubsystemDistributablePowerFraction (PoweredSubsystemType.Health, shieldsFraction);
			
		}

		
        /// <summary>
        /// Clamp the power ball inside the triangle.
        /// </summary>
		void ClampInsideTriangle()
		{

			// Clamp power ball inside the triangle
			powerBallPosition.y = Mathf.Clamp (powerBallPosition.y, -2/3f, 1/3f);
			float maxPosX = (powerBallPosition.y + 2/3f) * Mathf.Tan (30f * Mathf.Deg2Rad);
			powerBallPosition.x = Mathf.Clamp (powerBallPosition.x, -maxPosX, maxPosX);		

		}

		
		// Find the distance from a point to a line (Point (px,py) to Line y = mx + c)
		float DistToLine(float px, float py, float m, float c)
		{
			float numerator = Mathf.Abs (m * px - py + c);
			float denominator = Mathf.Sqrt(m*m + 1);
			return (numerator/denominator);
		}


		// Called every frame
		void Update()
		{
            
			if (!menuActivated || power == null) return;
			
			totalPowerText.text = "TOTAL: " + Mathf.RoundToInt(power.TotalPower).ToString() + " kW";
			
			enginesInfoController.SetPowerValues(power.GetSubsystemTotalPower(PoweredSubsystemType.Engines), power.TotalPower, 
													power.GetSubsystemDistributablePower(PoweredSubsystemType.Engines));

			weaponsInfoController.SetPowerValues(power.GetSubsystemTotalPower(PoweredSubsystemType.Weapons), power.TotalPower, 
													power.GetSubsystemDistributablePower(PoweredSubsystemType.Weapons));

			shieldsInfoController.SetPowerValues(power.GetSubsystemTotalPower(PoweredSubsystemType.Health), power.TotalPower, 
													power.GetSubsystemDistributablePower(PoweredSubsystemType.Health));

		}
	}
}