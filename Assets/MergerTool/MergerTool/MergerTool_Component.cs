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
    [ReadOnly] [SerializeField] private bool isStatic = false;

    [ReadOnly] [SerializeField] public MeshRegistry meshRegistry;

    public bool wasAddedManually = true;
    [ReadOnly] [SerializeField] private List<Vector3> uvList;

    private void Start()
    {
        InitializeComponent();
        //ConstructComponent(MergerTool.main.getData(ID, this));

        ////Load the new material here
        //if (null != customMaterial) { GetComponent<Renderer>().material = customMaterial; }

        //if( null != meshRegistry && isStatic)
        //{
        //    //Debug.Log("Checking Nearest In: " + gameObject.name);
        //    meshRegistry.MergeToRoot(gameObject, ID, prefabIndex, maximumDistanceToRoot);
        //}
    }

    private void InitializeComponent()
    {
        if (MergerTool.main.hasData(ID))
        { ApplyDataPacket(); }
        else
        {
            //Debug.Log("No data packet for ID '" + ID + "' observing if that changes!");
            MergerTool.packetObserver += HandleNewPacket;
        }
    }

    private void HandleNewPacket(string ID)
    {
        if(ID == this.ID)
        { 
            //Debug.Log("Observer for ID '" + ID + "' in object '" + gameObject.name + "' triggered!");
            ApplyDataPacket();
            MergerTool.packetObserver -= HandleNewPacket;
        }
    }

    private void ApplyDataPacket()
    {
        ConstructComponent(MergerTool.main.getData(ID, this));

        //Load the new material here
        if (null != customMaterial) { GetComponent<Renderer>().material = customMaterial; }
        MergeMesh();
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
                if(isStatic) { gameObject.isStatic = this.isStatic; }
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

        MeshFilter meshFilterComp = GetComponent<MeshFilter>();

        for (int i = 0; i < meshFilterComp.sharedMesh.uv.Length; i++)
        {
            uvList.Add(new Vector3(meshFilterComp.sharedMesh.uv[i].x,
                                   meshFilterComp.sharedMesh.uv[i].y, prefabIndex));
        }
        meshFilterComp.mesh.SetUVs(0, uvList);
    }

    //public void CombineMeshes()
    //{
    //    Vector3 originalPos = gameObject.transform.parent.position;
    //    gameObject.transform.parent.position = Vector3.zero;

    //    gameObject.transform.parent.GetComponent<MeshRenderer>().material = customMaterial;

    //    MeshFilter[] meshFilters = gameObject.transform.parent.GetComponentsInChildren<MeshFilter>();
    //    CombineInstance[] combine = new CombineInstance[meshFilters.Length];

    //    int i = 0;

    //    while(i < meshFilters.Length)
    //    {
    //        combine[i].mesh = meshFilters[i].sharedMesh;
    //        combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
    //        if (isStatic) { meshFilters[i].gameObject.SetActive(false); }
    //        else if (!isStatic && i > 0) { meshFilters[i].GetComponent<MeshRenderer>().enabled = false; }
    //        i++;
    //    }

    //    gameObject.transform.parent.GetComponent<MeshFilter>().mesh = new Mesh();
    //    gameObject.transform.parent.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);

    //    if(gameObject.transform.parent.GetComponent<MeshFilter>().mesh.vertexCount > vertexLimit) 
    //    { throw new System.Exception("!!! ERROR: object '" + gameObject.transform.parent.name + "' has exceeded vertex limit on combined mesh, it's going to look really weird !!!"); }

    //    if (isStatic) { gameObject.transform.parent.GetComponent<MeshCollider>().sharedMesh = gameObject.transform.parent.GetComponent<MeshFilter>().mesh; }

    //    gameObject.transform.parent.gameObject.SetActive(true);

    //    gameObject.transform.parent.position = originalPos;
    //}

    public void ReleaseMergedMesh()
    {
        Debug.Log("Called ReleaseMergedMesh on: " + gameObject.name);
        meshRegistry.DetachFromRoot(gameObject, ID, prefabIndex, maximumDistanceToRoot);
    }

    public void MergeMesh()
    {
        if (null != meshRegistry)
        {
            //Debug.Log("Checking Nearest In: " + gameObject.name);
            meshRegistry.MergeToRoot(gameObject, ID, prefabIndex, maximumDistanceToRoot);
        }
    }

    public bool IsStatic {  get { return isStatic; } }
    public Material CustomMaterial { get { return customMaterial; } }

}
