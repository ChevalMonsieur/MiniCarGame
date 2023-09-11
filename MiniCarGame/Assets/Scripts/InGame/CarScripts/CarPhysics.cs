using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPhysics : MonoBehaviour
{
    // Gameobjects and components
    Rigidbody rb;
    CarStats carStats;

    // variables
    float verticalRotation;
    float horizontalRotation;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        carStats = GetComponent<CarStats>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // update forces
        Frictions();
        AirControl();
    }

    void Frictions() {
        // physics friction
        rb.AddForce(-rb.velocity * rb.velocity.magnitude * carStats.friction * Time.fixedDeltaTime);

        // practical friction
        if (rb.velocity.magnitude > 0.2f)
        {
            rb.AddForce(-rb.velocity.normalized * carStats.arcadeFriction * Time.fixedDeltaTime);
        } else if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            rb.velocity = Vector3.zero;
        }
    }

    void AirControl() {
        if (!WheelPhysics.isCarGrounded)
        {            
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                horizontalRotation -= Time.deltaTime * carStats.steeringSpeedAir;
                if (horizontalRotation < -carStats.maxSteeringAngleAir) horizontalRotation = -carStats.maxSteeringAngleAir;
            }
            else if (horizontalRotation < 0)
            {
                horizontalRotation += Time.deltaTime * carStats.steeringSpeedAir * 2;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                horizontalRotation += Time.deltaTime * carStats.steeringSpeedAir;
                if (horizontalRotation < -carStats.maxSteeringAngleAir) horizontalRotation = -carStats.maxSteeringAngleAir;
            }
            else if (horizontalRotation > 0)
            {
                horizontalRotation -= Time.deltaTime * carStats.steeringSpeedAir * 2;
            }

            // clip steering
            if (horizontalRotation > -1f && horizontalRotation < 1f && !Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)) verticalRotation = 0;

            // apply steering
            transform.RotateAround(transform.position, transform.up, horizontalRotation * Time.deltaTime);


            if (Input.GetKey(KeyCode.DownArrow))
            {
                verticalRotation -= Time.deltaTime * carStats.steeringSpeedAir;
                if (verticalRotation < -carStats.maxSteeringAngleAir) verticalRotation = -carStats.maxSteeringAngleAir;
            }
            else if (verticalRotation < 0)
            {
                verticalRotation += Time.deltaTime * carStats.steeringSpeedAir * 2;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                verticalRotation += Time.deltaTime * carStats.steeringSpeedAir;
                if (verticalRotation < -carStats.maxSteeringAngleAir) verticalRotation = -carStats.maxSteeringAngleAir;
            }
            else if (verticalRotation > 0)
            {
                verticalRotation -= Time.deltaTime * carStats.steeringSpeedAir * 2;
            }

            // clip steering
            if (verticalRotation > -1f && verticalRotation < 1f && !Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.UpArrow)) verticalRotation = 0;

            // apply steering
            transform.RotateAround(transform.position, transform.right, verticalRotation * Time.deltaTime);
        }
    }
}
