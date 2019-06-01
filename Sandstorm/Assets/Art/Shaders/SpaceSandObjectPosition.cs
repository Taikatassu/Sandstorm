using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSandObjectPosition : MonoBehaviour
{
    public Material sand;

    void Update()
    {
        Vector3 p = transform.position;
        sand.SetVector("Position", new Vector4(p.x, p.y, p.z, 0));
    }
}
