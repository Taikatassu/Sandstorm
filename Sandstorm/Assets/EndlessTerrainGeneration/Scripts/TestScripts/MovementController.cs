using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Vector3 movementSpeed;
    public bool useLocalDirections = true;

    private Transform t;

    void Start()
    {
        t = transform;
    }

    void Update()
    {
        Vector3 movementVector = (useLocalDirections)
            ? (t.right * movementSpeed.x + t.up * movementSpeed.y + t.forward * movementSpeed.z)
            : movementSpeed;

        t.position += movementVector * Time.deltaTime;
    }
}
