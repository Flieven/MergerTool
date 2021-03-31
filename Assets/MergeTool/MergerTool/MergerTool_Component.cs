using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeTool;

[AddComponentMenu("MergerTool/MergerTool Component")]
public class MergerTool_Component : MonoBehaviour
{

    [SerializeField] public string ID;
    [ReadOnly] [SerializeField] public float maximumDistanceToRoot;
    [ReadOnly] [SerializeField] public Material customMaterial;
    [ReadOnly] [SerializeField] public int prefabIndex;
    [ReadOnly] [SerializeField] public bool isStatic;

    [ReadOnly] [SerializeField] public MeshRegistry meshRegistry;

    public bool wasAddedManually = true;
    [ReadOnly] public List<Vector3> uvList;

    private void Start()
    {
        ConstructComponent(MergerTool.main.getData(ID, this));

        //Load the new material here
        if (null != customMaterial) { GetComponent<Renderer>().material = customMaterial; }

        if( null != meshRegistry)
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
                this.isStatic = packet.prefabs[i].isStatic;
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
        Vector3 originalPos = gameObject.transform.position;
        gameObject.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;

        while(i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            if (isStatic) { meshFilters[i].gameObject.SetActive(false); }
            i++;
        }

        gameObject.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        gameObject.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, isStatic);
        gameObject.transform.gameObject.SetActive(true);

        gameObject.transform.position = originalPos;
    }

}
