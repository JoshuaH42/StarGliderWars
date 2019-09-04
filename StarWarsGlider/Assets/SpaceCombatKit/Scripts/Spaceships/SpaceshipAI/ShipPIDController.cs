using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PIDController
{
    public float proportionalCoefficient = 0.01f;
    public float integralCoefficient;
    public float derivativeCoefficient;

    protected float proportionalValue;
    protected float integralValue;
    protected float derivativeValue;

    public float integralInfluence = 1;

    public void SetError(float error, float errorChangeRate)
    {

        // Proportional
        proportionalValue = proportionalCoefficient * error;

        // Integral
        integralValue += integralInfluence * (integralCoefficient * error);
        integralValue = Mathf.Clamp(integralValue, -1, 1);

        // Derivative
        derivativeValue = derivativeCoefficient * errorChangeRate;

    }

    public void SetIntegralInfluence(float influence)
    {
        this.integralInfluence = influence;
    }

    public float GetControlValue()
    {
        return proportionalValue + (integralInfluence * integralValue) + derivativeValue;
    }
}

[System.Serializable]
public class PIDController3D
{
    public PIDController controllerXAxis = new PIDController();
    public PIDController controllerYAxis = new PIDController();
    public PIDController controllerZAxis = new PIDController();

    public enum Axis
    {
        X,
        Y,
        Z
    }

    public void SetError(Axis axis, float error, float errorChangeRate)
    {
        switch (axis)
        {
            case Axis.X:
                controllerXAxis.SetError(error, errorChangeRate);
                break;
            case Axis.Y:
                controllerYAxis.SetError(error, errorChangeRate);
                break;
            case Axis.Z:
                controllerZAxis.SetError(error, errorChangeRate);
                break;
        }
    }

    public void SetIntegralInfluence(PIDController3D.Axis axis, float influence)
    {
        switch (axis)
        {
            case Axis.X:
                controllerXAxis.SetIntegralInfluence(influence);
                break;
            case Axis.Y:
                controllerYAxis.SetIntegralInfluence(influence);
                break;
            case Axis.Z:
                controllerZAxis.SetIntegralInfluence(influence);
                break;
        }
    }

    public void SetIntegralInfluence(float influence)
    {
        controllerXAxis.SetIntegralInfluence(influence);
        controllerYAxis.SetIntegralInfluence(influence);
        controllerZAxis.SetIntegralInfluence(influence);
    }

    public float GetControlValue (Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                return controllerXAxis.GetControlValue();
            case Axis.Y:
                return controllerYAxis.GetControlValue();
            default:    // Z
                return controllerZAxis.GetControlValue();
        }
    }

    public Vector3 GetControlValues()
    {
        return new Vector3(controllerXAxis.GetControlValue(), controllerYAxis.GetControlValue(), controllerZAxis.GetControlValue());
    }
}

[System.Serializable]
public class ShipPIDController
{
    // Steering PID
    public PIDController3D steeringPIDController = new PIDController3D();

    public void SetSteeringError(PIDController3D.Axis axis, float error, float errorChangeRate)
    {
        steeringPIDController.SetError(axis, error, errorChangeRate);
    }

    public void SetSteeringIntegralInfluence(float influence)
    {
        steeringPIDController.SetIntegralInfluence(PIDController3D.Axis.X, influence);
        steeringPIDController.SetIntegralInfluence(PIDController3D.Axis.Y, influence);
        steeringPIDController.SetIntegralInfluence(PIDController3D.Axis.Z, influence);
    }

    public Vector3 GetSteeringControlValues()
    {
        return steeringPIDController.GetControlValues();
    }


    // Movement PID
    public PIDController3D movementPIDController = new PIDController3D();

    public void SetMovementError(PIDController3D.Axis axis, float error, float errorChangeRate)
    {
        movementPIDController.SetError(axis, error, errorChangeRate);
    }

    public void SetMovementIntegralInfluence(float influence)
    {
        movementPIDController.SetIntegralInfluence(PIDController3D.Axis.X, influence);
        movementPIDController.SetIntegralInfluence(PIDController3D.Axis.Y, influence);
        movementPIDController.SetIntegralInfluence(PIDController3D.Axis.Z, influence);
    }

    public Vector3 GetMovementControlValues()
    {
        return movementPIDController.GetControlValues();
    }

}
