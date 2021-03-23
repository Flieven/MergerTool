using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnSets
{
    public GameObject objectToSpawn;
    public int numObjectsToSpawn;
}

[RequireComponent(typeof(BoxCollider))]
public class Testing_Spawner : MonoBehaviour
{
    [SerializeField] private SpawnSets[] spawnArray = null;

    private BoxCollider boundingBox = null;

    // Start is called before the first frame update
    void Start()
    {
        boundingBox = GetComponent<BoxCollider>();

        for (int i = 0; i < spawnArray.Length; i++)
        {
            for (int ii = 0; ii < spawnArray[i].numObjectsToSpawn; ii++)
            {
                GameObject newRandomObj = spawnArray[i].objectToSpawn;

                GameObject newObj = Instantiate(newRandomObj,
                    new Vector3(Random.Range(boundingBox.bounds.min.x, boundingBox.bounds.max.x),
                    Random.Range(boundingBox.bounds.min.y, boundingBox.bounds.max.y),
                    Random.Range(boundingBox.bounds.min.z, boundingBox.bounds.max.z)),
                    Random.rotation);

                newObj.name = newRandomObj.name + " " + ii;
            }
        }
    }

}
