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

}
