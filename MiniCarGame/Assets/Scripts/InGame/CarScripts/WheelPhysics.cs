using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPhysics : MonoBehaviour
{
    // GameObjects and components
    Rigidbody rb;
    CarStats carStats;
    ArrayList wheels = new();

    // variables
    float lastLength;
    float springLength;
    float counterForce;
    bool frontwheel;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public static bool isCarGrounded;
    public static Vector3 groundContact;
    Vector3 dir;
    Vector3 suspensionForce;
    Vector3 pointVelocity;


    void Start()
    {
        wheels.Add(this);

        frontwheel = name[0] == 'F';
        carStats = transform.parent.parent.GetComponent<CarStats>();
        rb = transform.parent.parent.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Steering();
        GroundedCheck();
    }

    void FixedUpdate()
    {
        AngularFrictions();
        Suspensions();
        ForwardForces();
    }

    void Suspensions()
    {
        Debug.DrawRay(transform.position, -transform.up * (carStats.maxLength + carStats.wheelRadius), Color.yellow);
        Debug.DrawRay(transform.position, -transform.up * (carStats.minLength + carStats.wheelRadius), Color.red);

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, carStats.maxLength + carStats.wheelRadius))
        {
            // setup values
            isGrounded = true;
            springLength = hit.distance - carStats.wheelRadius;
            groundContact = hit.point;

            // setup spring force (suspension = stiffness * compression + damper * compression velocity)
            suspensionForce = ((carStats.springStiffness * (carStats.restLength - springLength)) + carStats.damperStiffness * ((lastLength - springLength) / Time.fixedDeltaTime)) * transform.up;

            // apply spring force
            rb.AddForceAtPosition(suspensionForce, hit.point);
        }
        else isGrounded = false;

        lastLength = springLength;
    }

    void ForwardForces()
    {
        // Check if accelerating
        if (Input.GetKey(KeyCode.UpArrow) && isGrounded)
        {
            // check if wheels corresponds to the transmission to apply force
            switch (carStats.transmission)
            {
                case Transmission.FWD:
                    if (frontwheel) rb.AddForceAtPosition(transform.forward * -1f * carStats.motorForce * carStats.motorPowerCurve.Evaluate(rb.velocity.magnitude) * Time.fixedDeltaTime, groundContact);
                    break;
                case Transmission.RWD:
                    if (!frontwheel) rb.AddForceAtPosition(transform.forward * -1f * carStats.motorForce * carStats.motorPowerCurve.Evaluate(rb.velocity.magnitude) * Time.fixedDeltaTime, groundContact);
                    break;
                case Transmission.AWD:
                    rb.AddForceAtPosition(transform.forward * -1f * carStats.motorForce * carStats.motorPowerCurve.Evaluate(rb.velocity.magnitude) * Time.fixedDeltaTime, groundContact);
                    break;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow) && isGrounded)
        {
            switch (carStats.transmission)
            {
                case Transmission.FWD:
                    if (frontwheel) rb.AddForceAtPosition(-transform.forward * -1f * carStats.brakeForce * Time.fixedDeltaTime, groundContact);
                    break;
                case Transmission.RWD:
                    if (!frontwheel) rb.AddForceAtPosition(-transform.forward * -1f * carStats.brakeForce * Time.fixedDeltaTime, groundContact);
                    break;
                case Transmission.AWD:
                    rb.AddForceAtPosition(-transform.forward * -1f * carStats.brakeForce * Time.fixedDeltaTime, groundContact);
                    break;
            }
        }
    }

    void Steering()
    {
        if (frontwheel)
        {
            Debug.DrawRay(transform.position, transform.forward, Color.green);

            //check inputs
            if (Input.GetKey(KeyCode.LeftArrow)) transform.RotateAround(transform.position, -transform.up, carStats.steeringSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.RightArrow)) transform.RotateAround(transform.position, transform.up, carStats.steeringSpeed * Time.deltaTime);
            else if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(transform.rotation.x, 0, transform.localRotation.z), 5f * Time.deltaTime);

            // clamp rotation
            if (transform.localRotation.eulerAngles.y > carStats.maxSteeringAngle && transform.localRotation.eulerAngles.y < carStats.maxSteeringAngle * 2) transform.localRotation = Quaternion.Euler(transform.rotation.x, carStats.maxSteeringAngle, transform.localRotation.z);
            else if (transform.localRotation.eulerAngles.y < 360 - carStats.maxSteeringAngle && transform.localRotation.eulerAngles.y > 360 - carStats.maxSteeringAngle * 2) transform.localRotation = Quaternion.Euler(transform.rotation.x, -carStats.maxSteeringAngle, transform.localRotation.z);
        }
    }

    void GroundedCheck() {
        isCarGrounded = true;

        foreach (WheelPhysics wheel in wheels)
        {
            if (!wheel.isGrounded) isCarGrounded = false;
        }
    }

    void AngularFrictions()
    {
        Debug.Log(rb.velocity.magnitude);

        if (isGrounded)
        {
            // determine grip to apply
            float gripCoefficient = carStats.grip;
            if (Input.GetKey(KeyCode.Space) && !frontwheel) gripCoefficient *= carStats.driftGrip;

            // calculate counter force depending on parallel velocity
            pointVelocity = rb.GetPointVelocity(transform.position);
            counterForce = -Vector3.Dot(transform.right, pointVelocity) / Time.fixedDeltaTime;

            // apply counter force
            rb.AddForceAtPosition(transform.right * counterForce * gripCoefficient, transform.position);
        }
    }

    public float GetSpringLength()
    {
        return springLength;
    }
}