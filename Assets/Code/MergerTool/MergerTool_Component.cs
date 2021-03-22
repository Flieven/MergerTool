using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergerTool_Component : MonoBehaviour
{
    [SerializeField] private string ID = "";
    [SerializeField] private Material customMaterial = null;
    [SerializeField] private int matIndex = 0;

    [SerializeField] private MaterialMaker materialMaker = null;
    [SerializeField] private MeshRegistry meshRegistry = null;

    private void Start()
    {
        //Load the new material here
        if (null != customMaterial) { GetComponent<Renderer>().material = customMaterial; }
    }

    public void ConstructComponent(MaterialMaker matMaker, MeshRegistry meshReg, string setID, int materailIndex)
    {
        ID = setID;
        matIndex = materailIndex;
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
                                   GetComponent<MeshFilter>().sharedMesh.uv[i].y, matIndex));
        }
        GetComponent<MeshFilter>().sharedMesh.SetUVs(0, uvList);
    }

}
