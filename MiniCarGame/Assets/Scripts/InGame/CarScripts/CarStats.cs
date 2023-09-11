using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStats : MonoBehaviour
{
    // variables
    // Suspension
    [SerializeField] public float restLength = 0.8f;
    [SerializeField] public float springTravel = 0.3f;
    [SerializeField] public float springStiffness = 33000f;
    [SerializeField] public float wheelRadius = 0.9f;
    [SerializeField] public float damperStiffness;

    [HideInInspector] public float maxLength;
    [HideInInspector] public float minLength;

    // Wheel visual
    [SerializeField] public AnimationCurve flyingWheelsPositionCurve;
    [SerializeField] public float flyingOffset;

    [HideInInspector] public float perimetreRoue;

    // Forward forces
    [SerializeField] public AnimationCurve motorPowerCurve;
    [SerializeField] public Transmission transmission;
    [SerializeField] public float motorForce;
    [SerializeField] public float brakeForce;

    // Frictions
    [SerializeField] public float friction;
    [SerializeField] public float arcadeFriction;
    [SerializeField] public float grip;
    [SerializeField] public float driftGrip;

    // Steering
    [SerializeField] public float maxSteeringAngle = 30.0f;
    [SerializeField] public float maxSteeringAngleAir = 30.0f;
    [SerializeField] public float steeringSpeed = 60.0f;
    [SerializeField] public float steeringSpeedAir = 60.0f;

    void Start() {
        maxLength = restLength + springTravel;
        minLength = restLength - springTravel;

        perimetreRoue = 2f * Mathf.PI * wheelRadius;
    }
}

public enum Transmission
{
    FWD,
    RWD,
    AWD
}
