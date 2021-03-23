using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergerTool_Component : MonoBehaviour
{

    [SerializeField] public string ID;
    [SerializeField] public Material customMaterial;
    [SerializeField] public int prefabIndex;

    [SerializeField] public MaterialMaker materialMaker;
    [SerializeField] public MeshRegistry meshRegistry;

    private void Start()
    {
        //Load the new material here
        if (null != customMaterial) { GetComponent<Renderer>().material = customMaterial; }

        if( null != meshRegistry)
        {
            //Debug.Log("Checking Nearest In: " + gameObject.name);
            meshRegistry.MergeToRoot(gameObject, ID, prefabIndex);
        }
    }

    public void ConstructComponent(MaterialMaker matMaker, MeshRegistry meshReg, string setID, int materailIndex)
    {
        ID = setID;
        prefabIndex = materailIndex;
        materialMaker = matMaker;
        meshRegistry = meshReg;
        customMaterial = materialMaker.getMaterial(ID);
        UpdateUVs();
    }

    public void DestroyComponent()
    {
        DestroyImmediate(this, true);
    }

    void UpdateUVs()
    {
        List<Vector3> uvList = new List<Vector3>();

        for (int i = 0; i < GetComponent<MeshFilter>().sharedMesh.uv.Length; i++)
        {
            uvList.Add(new Vector3(GetComponent<MeshFilter>().sharedMesh.uv[i].x,
                                   GetComponent<MeshFilter>().sharedMesh.uv[i].y, prefabIndex));
        }
        GetComponent<MeshFilter>().sharedMesh.SetUVs(0, uvList);
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
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        gameObject.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        gameObject.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        gameObject.transform.gameObject.SetActive(true);

        gameObject.transform.position = originalPos;
    }

}
