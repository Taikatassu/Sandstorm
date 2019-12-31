using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public KeyCode spawnButton = KeyCode.E;
    public float minSpawnScale = 1;
    public float maxSpawnScale = 1;
    public Vector3 spawnForce;

    private Transform t;

    private void Start()
    {
        t = transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(spawnButton))
        {
            GameObject spawnedObject = Instantiate(prefabToSpawn, t.position, t.rotation);
            spawnedObject.transform.localScale = Vector3.one * Random.Range(minSpawnScale, maxSpawnScale);
            Vector3 directionalForce = t.right * spawnForce.x + t.up * spawnForce.y + t.forward * spawnForce.z;
            spawnedObject.GetComponent<Rigidbody>()?.AddForce(directionalForce, ForceMode.Impulse);
        }
    }
}
