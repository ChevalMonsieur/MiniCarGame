using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelVisual : MonoBehaviour
{
    // GameObjects and components
    WheelPhysics wheelPhysics;
    CarStats carStats;

    // variables
    float timeFlying = 0f;
    bool frontwheel;
    Vector3 lastPosition;



    void Start()
    {
        frontwheel = transform.parent.name[0] == 'F';
        transform.localPosition = new Vector3(0, -0.15f, 0);

        wheelPhysics = transform.parent.gameObject.GetComponent<WheelPhysics>();
        carStats = transform.parent.parent.parent.gameObject.GetComponent<CarStats>();
    }

    void FixedUpdate()
    {
        UpdateWheelPosition();
        UpdateWheelRotation();
    }

    void UpdateWheelPosition()
    {

        //check if grounded
        if (wheelPhysics.isGrounded)
        {
            // setup values 
            timeFlying = 0f;

            // update position
            transform.localPosition = new Vector3(0, -wheelPhysics.GetSpringLength(), 0);
        }
        else
        {
            transform.localPosition = new Vector3(0, carStats.flyingWheelsPositionCurve.Evaluate(timeFlying) * carStats.maxLength * -1f - carStats.flyingOffset, 0);
            timeFlying += Time.fixedDeltaTime;
        }
    }

    void UpdateWheelRotation()
    {
        // setup values
        Vector3.Distance(transform.position, lastPosition);

        // update rotation
        if (Vector3.Dot(transform.position - lastPosition, transform.parent.forward) >= 0) transform.Rotate(Vector3.right, Vector3.Distance(transform.position, lastPosition) * 360f / carStats.perimetreRoue);
        else transform.Rotate(-Vector3.right, Vector3.Distance(transform.position, lastPosition) * 360f / carStats.perimetreRoue);
        lastPosition = transform.position;
    }
}
