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

    private Mesh myMesh = null;
    private Material myMaterial = null;
    private MeshFilter myMeshFilter = null;

    private Vector2[] uvs;
    private int uvLength;

    private void Awake()
    {
        myMaterial = GetComponent<Renderer>().material;
        myMeshFilter = GetComponent<MeshFilter>();
        myMesh = myMeshFilter.sharedMesh;
        uvs = myMesh.uv;
        uvLength = myMesh.uv.Length;
        transform.GetComponent<MeshRenderer>().enabled = false;
    }

    private void Start()
    {
        InitializeComponent();

        //ConstructComponent(MergerTool.main.getData(ID));
        // if (null != customMaterial) { myMaterial = customMaterial; }
        // MergeMesh();

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
        { MergerTool.main.RequestDataSet(OnDataPackReceived, ID); }
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
            MergerTool.main.RequestDataSet(OnDataPackReceived, ID);
            //ConstructComponent(MergerTool.main.getData(ID));
            MergerTool.packetObserver -= HandleNewPacket;
        }
    }

    private void OnDataPackReceived(DataPacket packet)
    {
        Debug.Log(gameObject.name + " received data packet: " + packet.ID);
        ConstructComponent(packet);
        if (null != customMaterial) { myMaterial = customMaterial; }
        MergeMesh();
    }

    public void ConstructComponent(DataPacket packet)
    {
        if(null == packet) { MergerTool.packetObserver += HandleNewPacket; return; }

        for (int i = 0; i < packet.prefabs.Length; i++)
        {
            if(packet.prefabs[i].prefabMesh == myMesh)
            { prefabIndex = i; }
        }

        maximumDistanceToRoot = packet.prefabs[prefabIndex].maximumDistanceToRoot;
        isStatic = packet.prefabs[prefabIndex].isStatic;
        if (isStatic) { gameObject.isStatic = this.isStatic; }
        customMaterial = packet.mergedMaterial;
        meshRegistry = packet.meshRegistry;

        StartCoroutine(UpdateUVs());
    }

    public void DestroyComponent()
    {
        DestroyImmediate(this, true);
    }

    IEnumerator UpdateUVs()
    {
        uvList = new List<Vector3>();

        for (int i = 0; i < uvLength; i++)
        {
            uvList.Add(new Vector3(uvs[i].x,
                                   uvs[i].y, prefabIndex));
        }
        myMeshFilter.mesh.SetUVs(0, uvList);
        yield return null;
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
