using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Testing_Spawner : MonoBehaviour
{

    [SerializeField] private int numObjectsToSpawn = 10;
    [SerializeField] private GameObject[] objectsArray = null;

    private BoxCollider boundingBox = null;

    // Start is called before the first frame update
    void Start()
    {
        boundingBox = GetComponent<BoxCollider>();

        for (int i = 0; i < numObjectsToSpawn; i++)
        {
            GameObject newRandomObj = objectsArray[Random.Range(0, objectsArray.Length)];

            Instantiate(newRandomObj,
                new Vector3(Random.Range(boundingBox.bounds.min.x, boundingBox.bounds.max.x),
                Random.Range(boundingBox.bounds.min.y, boundingBox.bounds.max.y),
                Random.Range(boundingBox.bounds.min.z, boundingBox.bounds.max.z)),
                Random.rotation);

        }

    }

}
