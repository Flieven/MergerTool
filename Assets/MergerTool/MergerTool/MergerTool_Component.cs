using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("MergerTool/MergerTool Component")]
public class MergerTool_Component : MonoBehaviour
{

    [SerializeField] public string ID;
    [ReadOnly] [SerializeField] private float maximumDistanceToRoot;
    [ReadOnly] [SerializeField] private Material customMaterial;
    [ReadOnly] [SerializeField] private int prefabIndex;
    [ReadOnly] [SerializeField] private bool isStatic = true;

    [ReadOnly] [SerializeField] public MeshRegistry meshRegistry;

    public bool wasAddedManually = true;
    [ReadOnly] [SerializeField] private List<Vector3> uvList;

    private void Start()
    {
        ConstructComponent(MergerTool.main.getData(ID, this));

        //Load the new material here
        if (null != customMaterial) { GetComponent<Renderer>().material = customMaterial; }

        if( null != meshRegistry && isStatic)
        {
            //Debug.Log("Checking Nearest In: " + gameObject.name);
            meshRegistry.MergeToRoot(gameObject, ID, prefabIndex, maximumDistanceToRoot);
        }
    }

    public void ConstructComponent(DataPacket packet)
    {
        for (int i = 0; i < packet.prefabs.Length; i++)
        {
            if (packet.prefabs[i].prefab.GetComponent<MeshFilter>().sharedMesh == gameObject.GetComponent<MeshFilter>().sharedMesh)
            {
                prefabIndex = i;
                maximumDistanceToRoot = packet.prefabs[i].maximumDistanceToRoot;
                isStatic = packet.prefabs[i].isStatic;
            }
        }
        customMaterial = packet.mergedMaterial;
        UpdateUVs();
    }

    public void DestroyComponent()
    {
        DestroyImmediate(this, true);
    }

    void UpdateUVs()
    {
        uvList = new List<Vector3>();

        for (int i = 0; i < GetComponent<MeshFilter>().sharedMesh.uv.Length; i++)
        {
            uvList.Add(new Vector3(GetComponent<MeshFilter>().sharedMesh.uv[i].x,
                                   GetComponent<MeshFilter>().sharedMesh.uv[i].y, prefabIndex));
        }
        GetComponent<MeshFilter>().mesh.SetUVs(0, uvList);
    }

    public void MergeMesh()
    {
        Vector3 originalPos = gameObject.transform.parent.position;
        gameObject.transform.parent.position = Vector3.zero;

        gameObject.transform.parent.GetComponent<MeshRenderer>().material = customMaterial;

        MeshFilter[] meshFilters = gameObject.transform.parent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;

        while(i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            if (isStatic) { meshFilters[i].gameObject.SetActive(false); }
            i++;
        }

        gameObject.transform.parent.GetComponent<MeshFilter>().mesh = new Mesh();
        gameObject.transform.parent.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, isStatic); 

        gameObject.transform.parent.gameObject.SetActive(true);

        gameObject.transform.parent.position = originalPos;
    }

    //public void UpdateMergeMesh()
    //{
    //    // FUTURE FEATURE TO ALLOW UPDATING THE PARENTED GROUPS OF NON-STATIC OBJECTS
    //    // Started doing this but realized I'd probably be better of finishing up for a presentable alpha, if you're reading this please ignore this feature as of right now.

    //    //if (null != meshRegistry && isStatic)
    //    //{
    //    //    //Debug.Log("Checking Nearest In: " + gameObject.name);
    //    //    Node nearest = meshRegistry.getNearest(gameObject, ID, prefabIndex, maximumDistanceToRoot);

    //    //    if(null == nearest)
    //    //    {
    //    //        Debug.Log("===== Updating Merge for: '" + gameObject.name + "' nearest returned null, assuming new Root established. =====");
    //    //    }
    //    //    else if(null != nearest)
    //    //    {
    //    //        GameObject oldParent = gameObject.transform.parent.gameObject;
    //    //    }
    //    //}
    //}

}
