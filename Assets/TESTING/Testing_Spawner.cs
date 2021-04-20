using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnSets
{
    public GameObject objectToSpawn;
    public int numObjectsToSpawn;
    [ReadOnly] public int totalNumSpawnedObjects;
}

public class Testing_Spawner : MonoBehaviour
{
    [SerializeField] bool showCube = false;
    [SerializeField] private Vector3 minCorner = Vector3.zero;
    [SerializeField] private Vector3 maxCorner = Vector3.zero;
    [Space]
    [SerializeField] private bool randomizedSpawn = true;
    [Space]
    [SerializeField] private SpawnSets[] spawnArray = null;


    private void MakeNewPacket()
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
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < spawnArray.Length; i++)
        {
            for (int ii = 0; ii < spawnArray[i].numObjectsToSpawn; ii++)
            {
                GameObject newRandomObj = spawnArray[i].objectToSpawn;

                GameObject newObj = null;

                if (!randomizedSpawn)
                {

                    newObj = Instantiate(newRandomObj,
                        new Vector3((ii * 2) + (spawnArray[i].totalNumSpawnedObjects * 2), i * 2, 0), Quaternion.identity);
                    //MergerTool.main.Add_MergeToolComponent(newObj, "CodePacket");
                    //if (i == 2)
                    //{
                    //    MergerTool.main.Add_MergeToolComponent(newObj, "CodePacket");
                    //}
                }
                else
                {
                    newObj = Instantiate(newRandomObj,
                             new Vector3(Random.Range(minCorner.x, maxCorner.x),
                                         Random.Range(minCorner.y, maxCorner.y),
                                         Random.Range(minCorner.z, maxCorner.z)),
                                         Random.rotation);
                }

                newObj.name = newRandomObj.name + " " + (ii + spawnArray[i].totalNumSpawnedObjects);

                if(ii >= spawnArray[i].numObjectsToSpawn -1) { spawnArray[i].totalNumSpawnedObjects += spawnArray[i].numObjectsToSpawn; }
            }
        }
        // if (null != MergerTool.main) { MakeNewPacket(); }
    }

    private void OnDrawGizmos()
    {
        if(showCube)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(minCorner, 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(maxCorner, 0.2f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(maxCorner + (minCorner - maxCorner) * 0.5f, minCorner - maxCorner);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            if (null != MergerTool.main) { MakeNewPacket(); }
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            SpawnObjects();
        }
    }
}
