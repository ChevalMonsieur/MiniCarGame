using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInGame : MonoBehaviour
{
    // components and gameobjects
    Transform toFollow;

    // variables
    [SerializeField] float rigidity;
    [SerializeField] Vector3 offset;

    Vector3 ignore;


    void Start() {
        toFollow = GameObject.Find("PlayerCar").transform;
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, toFollow.position + (toFollow.forward * offset.z + toFollow.right * offset.x + toFollow.up * offset.y), ref ignore, rigidity * Time.deltaTime);

        transform.LookAt(toFollow, Vector3.up);
    }
}
