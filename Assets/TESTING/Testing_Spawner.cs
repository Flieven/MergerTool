using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeTool;

[System.Serializable]
public struct SpawnSets
{
    public GameObject objectToSpawn;
    public int numObjectsToSpawn;
}

[RequireComponent(typeof(BoxCollider))]
public class Testing_Spawner : MonoBehaviour
{

    [SerializeField] private bool randomizedSpawn = true;
    [SerializeField] private SpawnSets[] spawnArray = null;

    private BoxCollider boundingBox = null;

    private void Awake()
    {
        PrefabStruct[] myStruct = new PrefabStruct[4]
        {
            new PrefabStruct(spawnArray[3].objectToSpawn, 5, true),
            new PrefabStruct(spawnArray[0].objectToSpawn, 15, true),
            new PrefabStruct(spawnArray[1].objectToSpawn, 15, true),
            new PrefabStruct(spawnArray[2].objectToSpawn, 15, true)
        };

        MergerTool.main.Add_DataPacket("CodePacket", myStruct);
    }

    // Start is called before the first frame update
    void Start()
    {
        boundingBox = GetComponent<BoxCollider>();

        for (int i = 0; i < spawnArray.Length; i++)
        {
            for (int ii = 0; ii < spawnArray[i].numObjectsToSpawn; ii++)
            {
                GameObject newRandomObj = spawnArray[i].objectToSpawn;

                GameObject newObj = null;

                if (!randomizedSpawn)
                {

                    newObj = Instantiate(newRandomObj,
                        new Vector3(ii * 2, i * 2, 0), Quaternion.identity);
                }
                else
                {
                    newObj = Instantiate(newRandomObj,
                             new Vector3(Random.Range(boundingBox.bounds.min.x, boundingBox.bounds.max.x),
                                         Random.Range(boundingBox.bounds.min.y, boundingBox.bounds.max.y),
                                         Random.Range(boundingBox.bounds.min.z, boundingBox.bounds.max.z)),
                                         Random.rotation);
                }

                newObj.name = newRandomObj.name + " " + ii;
            }
        }
    }

}
